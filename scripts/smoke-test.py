"""Run after a Release build: python scripts/smoke-test.py /path/to/dotnet.
Requires Python's requests package. Uses isolated test ports and a test-only admin.
No SQL Server is required; this checks HTTP/auth boundaries, not SQL CRUD.
"""
import os
from pathlib import Path
import re
import subprocess
import sys
import time
import requests
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
root = Path(__file__).resolve().parents[1]
dotnet = sys.argv[1] if len(sys.argv) > 1 else 'dotnet'
processes, logs = [], []
# This password/hash pair is public test data, never used by the application defaults.
password = 'SmokeTestOnly_2026!'
password_hash = 'AQAAAAIAAYagAAAAEEGc59i0U8O10tJnfKsI4WoJpdKsnLKKFO7sO9FPUMvh1VJy79YpaGKJigaXVfO7jQ=='
api, web = 'http://localhost:5290', 'https://localhost:5291'
def start(project, url, extra):
    env = dict(os.environ, ASPNETCORE_ENVIRONMENT='Development', ASPNETCORE_URLS=url, **extra)
    log = open(root / f'{project.replace("/", "-")}.smoke.log', 'w')
    logs.append(log)
    dll = root / project
    processes.append(subprocess.Popen([dotnet, str(dll)], cwd=dll.parent, env=env, stdout=log, stderr=log))
def wait(url):
    deadline = time.monotonic() + 40
    while time.monotonic() < deadline:
        if any(p.poll() is not None for p in processes): raise RuntimeError('Host exited; inspect smoke logs')
        try:
            if requests.get(url, verify=False, timeout=2).status_code == 200: return
        except requests.RequestException: pass
        time.sleep(.2)
    raise RuntimeError('Host did not become ready')
def check(response, expected):
    assert response.status_code == expected, (response.status_code, expected, response.text[:300])
try:
    start('Api/bin/Release/net10.0/MyPortfolio.Api.dll', api, {'Admin__UserName':'admin','Admin__PasswordHash':password_hash,'ConnectionStrings__Portfolio':'Server=127.0.0.1,1;Database=Smoke;User Id=test;Password=test;Encrypt=false;Connect Timeout=1'})
    start('MyPortfolio/bin/Release/net10.0/MyPortfolio.web.dll', web, {'PortfolioApi__BaseUrl':api+'/'})
    wait(api+'/health'); wait(web+'/')
    check(requests.get(api+'/api/admin/entries/section/Skills'), 401)
    check(requests.post(api+'/api/admin/entries', json={}), 401)
    check(requests.get(api+'/api/portfolio/999'), 400)
    login = requests.post(api+'/api/auth/login',json={'userName':'admin','password':password}); check(login,200)
    token = login.json()['accessToken']; headers={'Authorization':'Bearer '+token}
    me = requests.get(api+'/api/auth/me',headers=headers); check(me,200); assert me.json()['userName']=='admin'
    check(requests.post(api+'/api/admin/entries',headers=headers,json={'section':99,'title':'bad','summary':'bad','url':'javascript:alert(1)'}),400)
    check(requests.post(api+'/api/admin/entries',headers=headers,json={'section':1,'title':'','summary':'bad'}),400)
    failed_sql = requests.get(api+'/api/portfolio/Skills'); check(failed_sql,500)
    assert 'SqlException' not in failed_sql.text and 'Password=test' not in failed_sql.text
    session = requests.Session()
    page = session.get(web+'/account/login',verify=False); check(page,200)
    csrf = re.search(r'name="__RequestVerificationToken"[^>]*value="([^"]+)"', page.text).group(1)
    check(session.post(web+'/account/sign-in',data={'username':'admin','password':password},verify=False),400)
    response = session.post(web+'/account/sign-in',data={'username':'admin','password':password,'__RequestVerificationToken':csrf},verify=False,allow_redirects=False)
    check(response,302); assert response.headers['Location']=='/admin'
    cookie = response.headers['Set-Cookie'].lower(); assert 'secure' in cookie and 'httponly' in cookie and 'samesite=strict' in cookie
    dashboard = session.get(web+'/admin',verify=False); check(dashboard,200); assert 'Manage your portfolio' in dashboard.text
    check(session.get(web+'/admin/Skills/create',verify=False),200)
    public = requests.get(web+'/skills',verify=False); check(public,200); assert 'temporarily unavailable' in public.text
    assert token not in dashboard.text
    check(requests.post(api+'/api/auth/login',json={}),400)
    check(requests.post(api+'/api/auth/login',json={'userName':'admin','password':'incorrect'}),401)
    check(requests.post(api+'/api/auth/login',json={'userName':'admin','password':'incorrect'}),401)
    check(requests.post(api+'/api/auth/login',json={'userName':'admin','password':'incorrect'}),429)
    logout_csrf = re.search(r'name="__RequestVerificationToken"[^>]*value="([^"]+)"',dashboard.text).group(1)
    check(session.post(web+'/account/logout',data={'__RequestVerificationToken':logout_csrf},verify=False,allow_redirects=False),302)
    after = session.get(web+'/admin',verify=False,allow_redirects=False); assert after.status_code in (302,401)
    print('PASS: public rendering, API authentication, validation, HTTPS cookie sign-in/out, CSRF, protected pages, login rate limit, safe database failures')
finally:
    for p in processes: p.terminate()
    for p in processes:
        try: p.wait(timeout=5)
        except subprocess.TimeoutExpired: p.kill(); p.wait()
    for log in logs: log.close()

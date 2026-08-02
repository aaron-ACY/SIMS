process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
fetch('https://localhost:7096/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({username: 'admin', password: 'Admin123'})
}).then(async r => {
  const j = await r.json();
  const token = j.result.accessToken;
  const res = await fetch('https://localhost:7096/api/classes', {
    headers: { 'Authorization': 'Bearer ' + token }
  });
  console.log('classes:', await res.text());
  
  const res2 = await fetch('https://localhost:7096/api/subjects', {
    headers: { 'Authorization': 'Bearer ' + token }
  });
  console.log('subjects:', await res2.text());
  
  const res3 = await fetch('https://localhost:7096/api/majors', {
    headers: { 'Authorization': 'Bearer ' + token }
  });
  console.log('majors:', await res3.text());
}).catch(console.error);

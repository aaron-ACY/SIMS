const axios = require('axios');
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';
async function test() {
  try {
    const loginRes = await axios.post('https://localhost:7096/api/auth/login', { username: 'admin', password: 'Admin123!' });
    const token = loginRes.data.result.accessToken;
    const res = await axios.get('https://localhost:7096/api/classes', { headers: { Authorization: `Bearer ${token}` } });
    console.log(JSON.stringify(res.data.result, null, 2));
  } catch (err) {
    console.error(err.response ? err.response.data : err.message);
  }
}
test();

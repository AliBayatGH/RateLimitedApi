import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
        { duration: '10s', target: 50 },  // Ramp up to 50 virtual users over 10 second
        { duration: '30s', target: 50 },  // Stay at 50 virtual users for 30 second
    { duration: '20s', target: 0 }    // Ramp down to 0 virtual users over 20 second
  ],
  thresholds: {
    http_req_duration: ['p(95)<500'],  // 95% of requests must complete within 500ms
  },
};

export default function () {
  // Generate a random IP address for each virtual user
  let ipAddress = Math.floor(Math.random() * 255) + 1 + "." +
                  Math.floor(Math.random() * 255) + "." +
                  Math.floor(Math.random() * 255) + "." +
                  Math.floor(Math.random() * 255);

  // Make a request to the /login endpoint with the random IP address
    let res = http.get('https://localhost:7030/Login', { headers: { 'X-Forwarded-For': ipAddress } });
  
  // Check if the request was successful
  check(res, {
    'status is 200': (r) => r.status === 200,
  });
  
  // Sleep for a short random interval between requests
  sleep(Math.random() * 3);
}

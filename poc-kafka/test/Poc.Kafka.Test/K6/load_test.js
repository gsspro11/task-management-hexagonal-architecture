// Projeto utilizado para teste está em samples/ProducerApi exeutando local modo release.
import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
    vus: 10,
    duration: '1m',
    thresholds: {
        'http_req_duration': ['p(95)<500']
    },
    summaryTrendStats: ["avg", "min", "max", "p(50)", "p(95)", "p(99)"]
};

export default function () {
    let payload = JSON.stringify({
        "Cvv": "958",
        "Number": "5859 5789 2489 4589"
    });

    let params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    http.post("https://localhost:7032/send", payload, params);
    sleep(1);
}

import { useEffect } from 'react';

export default function Callback() {
    useEffect(() => {
        const urlParams = new URLSearchParams(window.location.search);
        const code = urlParams.get('code');
        console.log('Authorization Code:', code);
        console.log('urlParams: ',urlParams);

        if (code) {
            // Send code to backend
            fetch('http://localhost:5000/auth/github/callback', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ code }),
            })
            .then(response => response.json())
            .then(data => {
                console.log('data are: ',data);
                // Handle the response from your backend. Maybe store an authentication token or set user data.
            });
        }
    }, []);

    return <div>Processing GitHub login...</div>;
}
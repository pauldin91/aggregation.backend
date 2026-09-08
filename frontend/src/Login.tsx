
export default function Login() {
  const loginWithGitHub = () => {
    const clientID = "YOUR_GITHUB_CLIENT_ID";
    const redirectURI = "http://localhost:3000/auth/github/callback";
    window.location.href = `https://github.com/login/oauth/authorize?client_id=${clientID}&redirect_uri=${redirectURI}`;
  };

  return <button onClick={loginWithGitHub}>Login with GitHub</button>;
}

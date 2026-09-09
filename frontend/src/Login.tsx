
export default function Login() {
  const loginWithGitHub = () => {
    const clientID = import.meta.env.VITE_GITHUB_CLIENT_ID;
    const redirectURI = encodeURI(import.meta.env.VITE_GITHUB_REDIRECT_URI);
    window.location.href = `https://github.com/login/oauth/authorize?client_id=${clientID}&redirect_uri=${redirectURI}`;
  };

  return <button onClick={loginWithGitHub}>Login with GitHub</button>;
}

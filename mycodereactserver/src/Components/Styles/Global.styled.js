import { createGlobalStyle, keyframes } from "styled-components";

const waveAnimation = keyframes`
  0% {
    background-position: 0 0;
  }
  50% {
    background-position: 100% 0;
  }
  100% {
    background-position: 0 0;
  }
`;

const GlobalStyles = createGlobalStyle`
  :root {
    font-family: Inter, system-ui, Avenir, Helvetica, Arial, sans-serif;
    line-height: 1.5;
    font-weight: 500;
    font-synthesis: none;
    text-rendering: optimizeLegibility;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
  }

  body {
    margin: 0;
    min-width: 100vw;
    min-height: 100vh;
    background: linear-gradient(-45deg, rgba(40, 10, 50, 2.8) 5%, rgb(0, 47, 43) 75%);
    background-size: 400% 400%;
    animation: ${waveAnimation} 30s infinite linear;
    color: aliceblue;
  }

  h1 {
    font-size: 3.2em;
    line-height: 1.1;
  }

  .link {
    text-decoration: none;
    color: inherit;
  }
`;

export default GlobalStyles;

const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
      "/Authentication/*"
    ],
    target: "https://localhost:7231",
    secure: false
  }
]

module.exports = PROXY_CONFIG;

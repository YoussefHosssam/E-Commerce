import type { Config } from "tailwindcss";

export default {
  content: ["./index.html", "./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        paper: "#F5F5F5",
        paperAlt: "#F5F5F5",
        ink: "#343339",
        muted: "rgba(52, 51, 57, 0.62)",
        badge: "#343339",
        line: "rgba(52, 51, 57, 0.12)",
      },
      fontFamily: {
        serif: ['"NewYork"', "Georgia", "serif"],
        sans: ['"Inter"', '"Helvetica Neue"', "Arial", "sans-serif"],
      },
      boxShadow: {
        drawer: "0 24px 80px rgba(20,16,14,0.16)",
      },
    },
  },
  plugins: [],
} satisfies Config;

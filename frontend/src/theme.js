import { createSystem, defaultConfig, defineConfig } from "@chakra-ui/react"

const config = defineConfig({
  theme: {
    tokens: {
      colors: {
        ai: {
          50:  { value: "#eef3f6" },
          100: { value: "#cfe0e8" },
          200: { value: "#9fc1d1" },
          300: { value: "#6ea2ba" },
          400: { value: "#3d7d9e" },
          500: { value: "#1d4e6b" },
          600: { value: "#173f57" },
          700: { value: "#122f42" },
          800: { value: "#0c1f2c" },
          900: { value: "#061016" },
        },
        shu: {
          500: { value: "#c1440e" },
        },
        washi: {
          value: "#f8f4ec",
        },
      },
      fonts: {
        heading: { value: "'Noto Serif JP', serif" },
        body: { value: "'Inter', sans-serif" },
      },
    },
    semanticTokens: {
      colors: {
        "bg.canvas": { value: "{colors.washi}" },
        "fg.default": { value: "{colors.ai.800}" },
        "accent.default": { value: "{colors.ai.500}" },
        "accent.emphasis": { value: "{colors.shu.500}" },
      },
    },
  },
})

export const system = createSystem(defaultConfig, config)
import resolve from "@rollup/plugin-node-resolve";
import typescript from "@rollup/plugin-typescript";

export default [
  {
    input: "src/index.ts",
    external: ["@hotwired/turbo"],
    output: [
      {
        name: "NTurbo",
        file: "dist/nturbo.es5-umd.js",
        format: "umd",
        sourcemap: true,
        globals: {
          "@hotwired/turbo": "Turbo"
        }
      }
    ],
    plugins: [
      resolve(),
      typescript({ target: "es5", downlevelIteration: true })
    ],
    watch: {
      include: "src/**"
    }
  },
  {
    input: "src/index.ts",
    external: ["@hotwired/turbo"],
    output: [
      {
        name: "NTurbo",
        file: "dist/nturbo.es2017-umd.js",
        format: "umd",
        sourcemap: true,
        globals: {
          "@hotwired/turbo": "Turbo"
        }
      },
      {
        name: "NTurbo",
        file: "dist/nturbo.es2017-esm.js",
        format: "es",
        sourcemap: true
      }
    ],
    plugins: [
      resolve(),
      typescript()
    ],
    watch: {
      include: "src/**"
    }
  }
]

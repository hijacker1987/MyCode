import { fileURLToPath, URL } from "node:url";
import { defineConfig } from "vite";
import plugin from "@vitejs/plugin-react";
import svgr from "vite-plugin-svgr";
import fs from "fs";
import path from "path";
import child_process from "child_process";
import dotenv from "dotenv";

dotenv.config();

const baseFolder =
    process.env.APPDATA !== undefined && process.env.APPDATA !== ""
        ? `${process.env.APPDATA}/ASP.NET/https`
        : `${process.env.HOME}/.aspnet/https`;

const isNotWindowsBuild = process.env.APPDATA === undefined || process.env.APPDATA === "";

let certFilePath = path.join(baseFolder, `${certificateName}.pem`);
let keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (isNotWindowsBuild) {
    certFilePath = path.join("/app/certs", "mycode.pem");
    keyFilePath = path.join("/app/certs", "mycode.key");
}

const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
const certificateName = certificateArg ? certificateArg.groups.value : "mycodereactserver";

if (!certificateName) {
    console.error("Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.")
    process.exit(-1);
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    if (0 !== child_process.spawnSync("dotnet", [
        "dev-certs",
        "https",
        "--export-path",
        certFilePath,
        "--format",
        "Pem",
        "--no-password",
    ], { stdio: "inherit", }).status) {
        throw new Error("Could not create certificate.");
    }
}

export default defineConfig({
    plugins: [plugin(), svgr()],
    build: {
        sourcemap: true,
    },
    resolve: {
        alias: {
            '@': fileURLToPath(new URL("./src", import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/mycodeapp': {
                target: `${process.env.VITE_BACKEND_URL}`,
                secure: false
            },
        },
        port: Number(process.env.VITE_FRONTEND_PORT),
        https: {
            key: fs.readFileSync(keyFilePath),
            cert: fs.readFileSync(certFilePath),
        }
    }
});

const a = "/watcher/";
const c = "/codes/";
const u = "/account/";

//SERVICE
export const errorOccured = "/somethingWentWrong"

//USERS
export const uReg = `${u}register`;
export const uLogin = `${u}login`;
export const uUpdateOwn = `${u}updateUserData/`; //userId
export const uPwChange = `${u}pwchange`;

//CODES
export const cUpdate = `${c}updateCodeData/`; //codeId

//ADMIN
export const uList = `${a}users`;
export const cList = `${a}codes`;
export const uUpdate = `${a}updateUserData/`; //userId

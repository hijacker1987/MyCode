const a = "/watcher/";
const c = "/codes/";
const u = "/account/";

//SERVICE
export const errorOccured = "/somethingWentWrong"

//USERS
export const uReg = `${u}register`;
export const uLogin = `${u}login`;
export const uUpdateOwn = `${u}updateUserData`;
export const uPwChange = `${u}pwchange`;

//CODES
export const cReg = `${c}register`;
export const cOwn = `${c}my-codes`;
export const cUpdateOwn = `${c}updateCodeData/`; //codeId
export const cUpdate = `${c}updateCodeData/`;    //codeId

//ADMIN
export const uList = `${a}users`;
export const cList = `${a}codes`;
export const uUpdate = `${a}updateUserData/`;   //userId

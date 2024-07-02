const a = "admin/";
const ac = "account/";
const au = "auth/";
const b = "cncbot/";
const c = "codes/";
const s = "ws/message/";
const t = "token/";
const u = "users/";

//USERS
    //GET
    export const recentChuckNorris = `https://api.chucknorris.io/jokes/random`;
    export const getUser = `${u}getUser`;
    export const getUserId = `${u}getUserId`;

    //POST
    export const userLogin = `${u}login`;
    export const userRegistration = `${u}register`;

    //PATCH
    export const userUpdate = `${u}userUpdate`;
    export const changePassword = `${u}changePassword`;

    //DELETE
    export const deleteAccount = `${u}userDelete`;

//CODES
    //GET
    export const getCodesByUser = `${c}by-user`;
    export const getCodesByVisibility = `${c}by-visibility`;
    export const getCodesByUserId = `${c}code-`;            //{id}

    //POST
    export const codeRegistration = `${c}register`;

    //PUT
    export const codeUpdate = `${c}cupdate-`;               //{id}

    //DELETE
    export const deleteCode = `${c}cdelete-`;               //{id}

//ADMIN
    //GET
    export const getAllUsers = `${a}getUsers`;
    export const getAllCodes = `${a}getCodes`;
    export const userById = `${a}user-by-`;                 //{id}

    //PUT
    export const userSuperUpdate = `${a}aupdate-`;          //{id}
    export const codeSuperUpdate = `${a}acupdate-`;         //{id}
    export const changeRole = `${a}asupdate`;

    //DELETE
    export const deleteSuperUser = `${a}aduser-`;           //{id}
    export const deleteSuperCode = `${a}adcode-`;           //{id}

//CHAT
    //BOT - POST
    export const chatBot = `${b}botio`;

    //LIVE - GET
    export const getRoom = `${s}get-room`;
    export const getActRoom = `${s}get-active-room`;
    export const getAnyArc = `${s}getAnyArchived`;
    export const getOwn = `${s}getOwnArchived`;
    export const getMessage = `${s}getArchived-`;           //{id}
    export const getActive = `${s}getActive-`;              //{id}

    //PUT
    export const dropBack = `${s}uActive-`;                 //{id}

//SERVICE
    //GET
    export const primary2fa = `${au}basicsTwoFactor`;

    export const gitHubLogin = `${ac}github-login`;
    export const googleLogin = `${ac}google-login`;
    export const facebookLogin = `${ac}facebook-login`;
    export const gitHubAddon = `${ac}github-addon`;
    export const googleAddon = `${ac}google-addon`;
    export const facebookAddon = `${ac}facebook-addon`;

    //POST
    export const enable2fa = `${au}enableTwoFactor`;
    export const verify2fa = `${au}verifyTwoFactor`;
    export const disable2fa = `${au}disableTwoFactor`;

    //DELETE
    export const revoke = `${t}revoke`;

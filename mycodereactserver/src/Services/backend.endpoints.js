const a = "admin/";
const au = "auth/";
const c = "codes/";
const u = "users/";

//USERS
    //GET
    export const recentChuckNorris = `https://api.chucknorris.io/jokes/random`;
    export const getUser = `${u}getUser`;                      //Authorized to admin too
    export const getUserId = `${u}getUserId`;                  //Authorized to admin too

    //POST
    export const userLogin = `${u}login`;
    export const userRegistration = `${u}register`;

    //PATCH
    export const userUpdate = `${u}user-`;                  //{id}
    export const changePassword = `${u}changePassword`;         //Authorized to admin too

    //DELETE
    export const deleteAccount = `${u}delete-`;             //{id}

//CODES
    //GET
    export const getCodesByUser = `${c}by-user`;                //Authorized to admin too
    export const getCodesByVisibility = `${c}by-visibility`;    //Authorized to admin too
    export const getCodesByUserId = `${c}code-`;           //{id} Authorized to admin too

    //POST
    export const codeRegistration = `${c}register`;             //Authorized to admin too

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

    //DELETE
    export const deleteSuperUser = `${a}aduser-`;           //{id}
    export const deleteSuperCode = `${a}adcode-`;           //{id}

//SERVICE
    export const revoke = `token/revoke`;
    export const primary2fa = `${au}basicsTwoFactor`;
    export const enable2fa = `${au}enableTwoFactor`;
    export const verify2fa = `${au}verifyTwoFactor`;
    export const disable2fa = `${au}disableTwoFactor`;
    export const reliableAdd = `${au}addReliableAddress`;

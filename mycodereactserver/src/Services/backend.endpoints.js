const a = "admin/"
const c = "codes/"
const u = "users/"

//USERS
    //GET
    export const recentChuckNorris = `https://api.chucknorris.io/jokes/random`;
    export const getUser = `${u}getUser`;                      //Authorized to admin too

    //POST
    export const userLogin = `${u}login`;
    export const userRegistration = `${u}register`;

    //PATCH
    export const userUpdate = `${u}u-`; //{id}
    export const changePassword = `${u}changePassword`;         //Authorized to admin too

    //DELETE
    export const deleteAccount = `${u}delete`;

//CODES
    //GET
    export const getCodesByUser = `${c}by-user`;                //Authorized to admin too
    export const getCodesByVisibility = `${c}by-visibility`;    //Authorized to admin too
    export const getCodesByUserId = `${c}ci-`; //{id}             Authorized to admin too

    //POST
    export const codeRegistration = `${c}register`;             //Authorized to admin too

    //PUT
    export const codeUpdate = `${c}cu-`; //{id}

    //DELETE
    export const deleteCode = `${c}cd-`; //{id}

//ADMIN
    //GET
    export const getAllUsers = `${a}getUsers`;
    export const getAllCodes = `${a}getCodes`;
    export const userById = `${a}user-by-`; //{id}

    //PUT
    export const userSuperUpdate = `${a}au-`;  //{id}
    export const codeSuperUpdate = `${a}acu-`; //{id}

    //DELETE
    export const deleteSuperCode = `${a}ad-`; //{id}
import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { postApi } from "../Services/Api";
import { ErrorPage, Notify } from "./Services";
import { userRegistration } from "../Services/Backend.Endpoints";
import UserForm from "../Components/Forms/UserForm/index";
import Loading from "../Components/Loading/index";

const UserRegister = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    const handleCreateUser = (user) => {
        setLoading(true);
        
        if (user.displayname == "") {
            user.displayname = "Anonymous";
        } 
        if (user.phoneNumber == "") {
            user.phoneNumber = "No phone number provided";
        }

        postApi(userRegistration, user)
            .then((res) => {
                setLoading(false);
                navigate("/");

                if (res.status != 400) {
                    if (res.email == undefined) {
                        Notify("Error", "Unable to Register, e-mail already in use!");
                    } else {
                        Notify("Success", `Successful Registration for ${res.email}!`);
                    }
                } else {
                    Notify("Error", "Unable to Register!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during registration: ${error}`);
            });
    };

    const handleCancel = () => {
        navigate(-1);
    };

    if (loading) {
        return <Loading />;
    }

    return <div>
                {errorMessage == "" ? (
                                      <UserForm onSave={handleCreateUser} onCancel={handleCancel} />
                                  ) : (
                                      <ErrorPage errorMessage={errorMessage} />
                                  )}
           </div>
};

export default UserRegister;

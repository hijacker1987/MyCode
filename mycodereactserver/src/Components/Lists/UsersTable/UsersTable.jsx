import React from 'react';

const UsersTable = ({ users, headers }) => {

    const formatElapsedTime = (lastTimeLogin) => {
        const lastTimeLoginDate = new Date(lastTimeLogin);
        const now = new Date();

        const elapsedTime = now - lastTimeLoginDate;
        const days = Math.floor(elapsedTime / (24 * 60 * 60 * 1000));
        const hours = Math.floor((elapsedTime % (24 * 60 * 60 * 1000)) / (60 * 60 * 1000));
        const minutes = Math.floor((elapsedTime % (60 * 60 * 1000)) / (60 * 1000));

        const dayFormat = days <= 1 ? "day" : "days";
        const hourFormat = hours <= 1 ? "hour" : "hours";
        const minuteFormat = minutes <= 1 ? "minute" : "minutes";

        return `${days} ${dayFormat}, ${hours} ${hourFormat}, ${minutes} ${minuteFormat} ago`;
    };

    return (
        <div className="table-responsive">
            <table className="table table-striped table-hover">
                <thead>
                    <tr>
                        {headers.map(header => (
                            <th key={header} className='table-primary' style={{ color: "darkred" }}>{header}</th>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {users && users.map((user, index) => (
                        <tr key={user.userName} style={{ color: "black", backgroundColor: index % 2 === 1 ? '#f2f2f2' : '' }}>
                            <td>{user.displayName}</td>
                            <td>{formatElapsedTime(user.lastTimeLogin)}</td>
                            <td>{user.userName}</td>
                            <td>{user.email}</td>
                            <td>{user.phoneNumber}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default UsersTable;

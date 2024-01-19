export const formatElapsedTime = (lastTimeLogin) => {
    const lastTimeLoginDate = new Date(lastTimeLogin);
    const now = new Date();

    const elapsedTime = now - lastTimeLoginDate;
    const days = Math.floor(elapsedTime / (24 * 60 * 60 * 1000));
    const hours = Math.floor((elapsedTime % (24 * 60 * 60 * 1000)) / (60 * 60 * 1000));
    const minutes = Math.floor((elapsedTime % (60 * 60 * 1000)) / (60 * 1000));

    const dayFormat = days <= 1 ? "day" : "days";
    const hourFormat = hours <= 1 ? "hour" : "hours";
    const minuteFormat = minutes <= 1 ? "minute" : "minutes";

    let result = "";

    if (days == 0 && hours == 0 && minutes == 0) {
        result = "Just now";
    }
    else {
        result = `${days} ${dayFormat}, ${hours} ${hourFormat}, ${minutes} ${minuteFormat} ago`;
    }

    return result;
};
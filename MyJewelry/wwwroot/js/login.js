// function getUserDetails(event) {
//     event.preventDefault();
//     const user = {
//         Id: Number(document.getElementById('userId').value),
//         Name: document.getElementById('userName').value
//     };

//     fetch('/api/Login/login', {
//         method: 'POST',
//         headers: { 'Content-Type': 'application/json' },
//         body: JSON.stringify(user)
//     })
//     .then(res => {
//         if (!res.ok) throw new Error("Invalid login");
//         return res.text();
//     })
//     .then(token => {
//         localStorage.setItem("token", token);
//         localStorage.setItem("userName", user.Name);
//         // We decode role logic simplified for demo:
//         const role = (user.Name === "Esty" && user.Id === 1272) ? "Admin" : "User";
//         localStorage.setItem("userRole", role);
//         window.location.href = '../index.html';
//     })
//     .catch(err => {
//         document.getElementById('login-msg').innerText = "Access Denied: User not found.";
//     });
// }

const getUserDetails = (event) => {
    event.preventDefault();

    // שליפת הנתונים מהטופס
    const user = {
        Id: Number(document.getElementById('userId').value),
        Name: document.getElementById('userName').value,
        Password: "", // ה-Controller מצפה לאובייקט User, אז צריך לספק את השדות
        Role: ""
    };

    console.log("asdfg:  "+JSON.stringify(user));
    

    // הנתיב המעודכן (תואם ל-Route ב-C#)
    const uri = '/api/Login/login'; 

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
    })
    .then(response => {
        if (response.status==401) {
            throw new Error("משתמש לא רשום במערכת, אנא פנה למנהל" + response.status + ")");
        }
        else if (!response.ok) {
            // אם קיבלת שגיאה 404 או 400 זה יקפוץ לכאן
            throw new Error("נתיב לא נמצא או נתונים שגויים (סטטוס: " + response.status + ")");
        }
        return response.text(); 
    })
    .then(token => {
        const cleanToken = token.replace(/"/g, ''); 
        localStorage.setItem("token", cleanToken);
        alert("התחברת בהצלחה!");
        window.location.href = '../index.html';
    })
    .catch(error => {
        console.error('Error:', error);
        alert("שגיאה: " + error.message);
    });
}

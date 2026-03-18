function getUserDetails(event) {
    event.preventDefault();
    const user = {
        Id: Number(document.getElementById('userId').value),
        Name: document.getElementById('userName').value
    };

    fetch('/api/Login/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
    })
    .then(res => {
        if (!res.ok) throw new Error("Invalid login");
        return res.text();
    })
    .then(token => {
        localStorage.setItem("token", token);
        localStorage.setItem("userName", user.Name);
        // We decode role logic simplified for demo:
        const role = (user.Name === "Esty" && user.Id === 1272) ? "Admin" : "User";
        localStorage.setItem("userRole", role);
        window.location.href = '../index.html';
    })
    .catch(err => {
        document.getElementById('login-msg').innerText = "Access Denied: User not found.";
    });
}

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


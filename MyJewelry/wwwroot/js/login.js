const getUserDetails = (event) => {
    event.preventDefault();

    let userName = document.getElementById('userName').value.trim();
    let userId = document.getElementById('userId').value.trim();

    const user = {
        Id: Number(userId),
        Name: userName,
        Age: 0,
        Gender: 'jhg',
    }

    const uri = '/User/Login';

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error("Login failed");
        }
        return response.json();
    })
    .then((data) => {
        localStorage.setItem("token", JSON.stringify(data));
        window.location.href = '../index.html';
    })
    .catch(error => console.error('Unable to login.', error));
}
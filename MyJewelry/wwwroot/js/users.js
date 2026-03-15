const uri = '/User';
let users = [];

function getItems() {
    const token = localStorage.getItem("token");

    fetch(uri, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const token = localStorage.getItem("token"); // שליפת הטוקן
    const addNameTextbox = document.getElementById('add-name');
    const addIdTextbox = document.getElementById('add-id');

    const user = {
        name: addNameTextbox.value.trim(),
        id: parseInt(addIdTextbox.value.trim(), 10), // חשוב שזה יהיה מספר
        password: '', // וודאי שהשרת מאפשר סיסמה ריקה ביצירה
        Role: "User"
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`, // הוספת הטוקן כאן!
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
    })
        .then(response => {
            if (!response.ok) throw new Error("Insert failed: " + response.statusText);
            return response.json();
        })
        .then(() => {
            getItems();
            addNameTextbox.value = '';
            addIdTextbox.value = '';
        })
        .catch(error => console.error('Unable to post.', error));
}
// function addItem() {
//     const addNameTextbox = document.getElementById('add-name');
//         const addIdTextbox = document.getElementById('add-id');
//     const addPasswordTextbox = document.getElementById('add-password');


//     const user = {
//         name: addNameTextbox.value.trim(),
//         id: addIdTextbox.value.trim(),
//         password: '',
//         Role: "User"
//     };


//     fetch(uri, {
//             method: 'POST',
//             headers: {
//                 'Accept': 'application/json',
//                 'Content-Type': 'application/json'
//             },
//             body: JSON.stringify(user)
//         })
//         .then(response => response.json())
//         .then(() => {
//             getItems();
//             addNameTextbox.value = '';
//         })
//         .catch(error => console.error('Unable to post.', error));
// }

// function deleteItem(id) {
//     fetch(`${uri}/${id}`, {
//         method: 'DELETE',
//         headers: {
//             'Authorization': `Bearer ${token}` // הוספת הטוקן
//         }
//     })
//         .then(() => getItems())
//         .catch(error => console.error('Unable to delete user.', error));
// }

function deleteItem(id) {
    const token = localStorage.getItem("token");
    
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}` // הוספת הטוקן
        }
    })
    .then(response => {
        if (!response.ok) throw new Error("Delete failed");
        getItems();
    })
    .catch(error => console.error('Unable to delete user.', error));
}

function displayEditForm(id) {
    const user = users.find(u => u.id === id);

    document.getElementById('edit-id').value = user.id;
    document.getElementById('edit-name').value = user.name;
    document.getElementById('edit-password').value = user.password;
    document.getElementById('editForm').style.display = 'block';
}

// function updateItem() {
//     const userId = document.getElementById('edit-id').value.trim();
//     console.log(userId);
//     const user = {
//         id: parseInt(userId, 10),
//         name: document.getElementById('edit-name').value.trim(),
//         password: document.getElementById('edit-password').value.trim(),
//     };


//     fetch(`${uri}/${userId}`, {
//         method: 'PUT',
//         headers: {
//             'Accept': 'application/json',
//             'Content-Type': 'application/json',
//                 'Authorization': `Bearer ${token}` // הוספת הטוקן
            
//         },
//         body: JSON.stringify(user)
//     })
//         .then(() => getItems())
//         .catch(error => console.error('Unable to update user.', error));

//     closeInput();

//     return false;
// }
// function updateItem() {
//     const token = localStorage.getItem("token");
//     const userId = document.getElementById('edit-id').value.trim();
    
//     const user = {
//         id: parseInt(userId),
//         name: document.getElementById('edit-name').value.trim(),
//         password: document.getElementById('edit-password').value.trim(),
//     };

//     fetch(`${uri}/${userId}`, {
//         method: 'PUT',
//         headers: {
//             'Authorization': `Bearer ${token}`, // הוספת הטוקן
//             'Accept': 'application/json',
//             'Content-Type': 'application/json'
//         },
//         body: JSON.stringify(user)
//     })
//     .then(response => {
//         if (!response.ok) throw new Error("Update failed");
//         getItems();
//     })
//     .catch(error => console.error('Unable to update user.', error));

//     closeInput();
//     return false;
// }

function updateItem() {
    const userId = document.getElementById('edit-id').value;
    
    const user = {
        id: parseInt(userId), // חייב להיות מספר ותואם ל-ID ב-URL
        name: document.getElementById('edit-name').value.trim(),
        // וודא שאתה שולח את כל השדות שהמודל ב-C# מצפה להם (Password, Role וכו')
        password: "", 
        role: "User"
    };

    fetch(`${uri}/${userId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}` // וודא שיש טוקן
        },
        body: JSON.stringify(user)
    })
    .then(response => {
        if (!response.ok) {
            // אם קיבלנו 400, ננסה להבין למה מהשרת
            return response.text().then(text => { throw new Error(text || "Update failed") });
        }
        return response;
    })
    .then(() => {
        getItems();
        closeInput();
    })
    .catch(error => console.error('Unable to update user.', error));
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function displayCount(userCount) {
    const name = (userCount === 1) ? 'user:' : 'users :';

    document.getElementById('counter').innerText = `${userCount} ${name}`;
}


function displayItems(data) {
    const tBody = document.getElementById('user');
    tBody.innerHTML = '';

    displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(user => {

        console.log(user + "----");


        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${user.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${user.id})`);

        let tr = tBody.insertRow();


        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(user.id);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNode2 = document.createTextNode(user.name);
        td2.appendChild(textNode2);

        let td3 = tr.insertCell(2);
        let textNode3 = document.createTextNode(user.password);
        td3.appendChild(textNode3);

        let td4 = tr.insertCell(3);
        td4.appendChild(editButton);

        let td5 = tr.insertCell(4);
        td5.appendChild(deleteButton);
    });

    users = data;
}
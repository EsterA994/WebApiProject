// const userUri = '/User';
// const token = localStorage.getItem("token");

// function showToast(msg) {
//     const t = document.getElementById('toast');
//     t.innerText = msg; t.classList.remove('hidden');
//     setTimeout(() => t.classList.add('hidden'), 3000);
// }

// // טעינת הדף
// async function loadProfilePage() {
//     await getMyProfile(); // כולם מקבלים את הפרטים שלהם
//     if (localStorage.getItem("userRole") === "Admin") {
//         document.getElementById('admin-section').classList.remove('hidden');
//         getAllUsers(); // רק אדמין מקבל את הרשימה המלאה
//     }
// }

// // 1. קבלת פרופיל אישי (endpoint: /User/me)
// function getMyProfile() {
//     fetch(`${userUri}/me`, {
//         headers: { 'Authorization': `Bearer ${token}` }
//     })
//     .then(res => res.json())
//     .then(user => {
//         document.getElementById('my-id').value = user.id;
//         document.getElementById('my-name').value = user.name;
//         document.getElementById('profile-title').innerText = `Hello, ${user.name}`;
//     });
// }

// // 2. עדכון פרטים אישיים
// function updateMyProfile(event) {
//     event.preventDefault();
//     const userId = document.getElementById('my-id').value;
//     const updatedUser = {
//         id: parseInt(userId),
//         name: document.getElementById('my-name').value,
//         password: document.getElementById('my-password').value || "", // סיסמה חדשה
//         role: localStorage.getItem("userRole")
//     };

//     fetch(`${userUri}/${userId}`, {
//         method: 'PUT',
//         headers: {
//             'Authorization': `Bearer ${token}`,
//             'Content-Type': 'application/json'
//         },
//         body: JSON.stringify(updatedUser)
//     })
//     .then(res => {
//         if (!res.ok) throw new Error();
//         showToast("Profile updated!");
//         localStorage.setItem("userName", updatedUser.name); // עדכון השם המקומי
//     })
//     .catch(() => showToast("Failed to update profile"));
// }

// // 3. קבלת כל המשתמשים (רק לאדמין)
// function getAllUsers() {
//     fetch(userUri, { // קריאת ה-GET הרגילה שמחזירה רשימה
//         headers: { 'Authorization': `Bearer ${token}` }
//     })
//     .then(res => res.json())
//     .then(data => {
//         const tBody = document.getElementById('user-list');
//         tBody.innerHTML = '';
//         data.forEach(u => {
//             let tr = tBody.insertRow();
//             tr.innerHTML = `
//                 <td>${u.id}</td>
//                 <td>${u.name}</td>
//                 <td>${u.role}</td>
//                 <td><button class="btn-delete" onclick="deleteUser(${u.id})">Delete</button></td>
//             `;
//         });
//     });
// }

// function deleteUser(id) {
//     if (id == document.getElementById('my-id').value) {
//         showToast("You can't delete yourself!");
//         return;
//     }
//     fetch(`${userUri}/${id}`, {
//         method: 'DELETE',
//         headers: { 'Authorization': `Bearer ${token}` }
//     })
//     .then(() => { showToast("User removed"); getAllUsers(); });
// }

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
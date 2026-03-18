const userUri = '/User';
const token = localStorage.getItem("token");
let myCurrentId = null;

function logout() {
    localStorage.clear();
    window.location.href = 'login.html';
}

const isAdmin = () => localStorage.getItem("userRole") === "Admin";

function showToast(msg) {
    const t = document.getElementById('toast');
    if(t) {
        t.innerText = msg;
        t.classList.remove('hidden');
        setTimeout(() => t.classList.add('hidden'), 3000);
    }
}

async function loadProfilePage() {
    if (!token) {
        window.location.href = 'login.html';
        return;
    }

    // שלב 1: טעינת הפרופיל שלי
    await getMyProfile(); 

    // שלב 2: אם אדמין, הצגת אזור ניהול וטעינת הטבלה
    if (isAdmin()) {
        const adminSection = document.getElementById('admin-section');
        if(adminSection) adminSection.classList.remove('hidden');
        getAllUsers(); 
    }
}

async function getMyProfile() {
    try {
        const res = await fetch(`${userUri}/me`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });
        if (res.status === 401) logout();
        const user = await res.json();
        
        myCurrentId = user.id; // שמירת ה-ID למניעת מחיקה עצמית בטבלה
        
        document.getElementById('my-id').value = user.id;
        document.getElementById('my-name').value = user.name;
        document.getElementById('profile-title').innerText = `Hello, ${user.name} (${user.role})`;
    } catch (err) {
        console.error("Error fetching profile:", err);
    }
}

// חיפוש משתמש לפי ID
function getUserById() {
    const searchInput = document.getElementById('search-id-input');
    const id = searchInput.value;
    
    if (!id) {
        getAllUsers(); 
        return;
    }

    fetch(`${userUri}/${id}`, {
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(res => {
        if (!res.ok) {
            if (res.status === 404) throw new Error("User ID not found in system");
            if (res.status === 403) throw new Error("You don't have permission to view this user");
            throw new Error("Search failed");
        }
        return res.json();
    })
    .then(user => {
        displayUsersInTable([user]);
    })
    .catch(err => {
        showToast(err.message);
        document.getElementById('user-list').innerHTML = `<tr><td colspan="4" style="text-align:center;">${err.message}</td></tr>`;
    });
}

function getAllUsers() {
    fetch(userUri, { 
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(res => res.json())
    .then(data => displayUsersInTable(data))
    .catch(err => console.error("Error fetching users:", err));
}

function displayUsersInTable(users) {
    const tBody = document.getElementById('user-list');
    if(!tBody) return;
    
    tBody.innerHTML = '';
    users.forEach(u => {
        let tr = tBody.insertRow();
        const isMe = (u.id == myCurrentId);

        tr.innerHTML = `
            <td>${u.id}</td>
            <td>${u.name}</td>
            <td><span class="badge">${u.role}</span></td>
            <td>
                ${isMe ? '<strong>You</strong>' : `<button class="btn-delete" onclick="deleteUser(${u.id})">Delete</button>`}
            </td>
        `;
    });
}

function addUser(event) {
    event.preventDefault();
    const newUser = {
        Name: document.getElementById('add-user-name').value,
        Password: document.getElementById('add-user-password').value,
        Role: document.getElementById('add-user-role').value
    };

    fetch(userUri, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(newUser)
    })
    .then(res => {
        if (!res.ok) throw new Error("Failed to add user");
        showToast("User added successfully");
        document.getElementById('add-user-form').reset();
        getAllUsers(); 
    })
    .catch(err => showToast(err.message));
}

function deleteUser(id) {
    if (!confirm("Are you sure?")) return;
    fetch(`${userUri}/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(res => {
        if(res.ok) { showToast("User removed"); getAllUsers(); }
    });
}

function updateMyProfile(event) {
    event.preventDefault();
    const updatedUser = {
        Id: parseInt(myCurrentId),
        Name: document.getElementById('my-name').value,
        Password: document.getElementById('my-password').value || "", 
        Role: localStorage.getItem("userRole")
    };

    fetch(`${userUri}/${myCurrentId}`, {
        method: 'PUT',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(updatedUser)
    })
    .then(res => {
        if (!res.ok) throw new Error("Update failed");
        showToast("Profile updated!");
        if (isAdmin()) getAllUsers();
    })
    .catch(err => showToast("Error updating profile"));
}


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



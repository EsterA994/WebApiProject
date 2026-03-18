const userUri = '/User';
const token = localStorage.getItem("token");

// פונקציית עזר לבדיקה אם המשתמש הנוכחי הוא אדמין
const isAdmin = () => localStorage.getItem("userRole") === "Admin";

function showToast(msg) {
    const t = document.getElementById('toast');
    if(t) {
        t.innerText = msg; t.classList.remove('hidden');
        setTimeout(() => t.classList.add('hidden'), 3000);
    } else {
        alert(msg);
    }
}

// טעינת הדף
async function loadProfilePage() {
    if (!token) {
        window.location.href = 'login.html';
        return;
    }

    await getMyProfile(); 

    if (isAdmin()) {
        const adminSection = document.getElementById('admin-section');
        if(adminSection) adminSection.classList.remove('hidden');
        getAllUsers(); 
    }
}

// 1. קבלת פרופיל אישי
function getMyProfile() {
    fetch(`${userUri}/me`, {
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(res => {
        if (res.status === 401) logout();
        return res.json();
    })
    .then(user => {
        document.getElementById('my-id').value = user.id;
        document.getElementById('my-name').value = user.name;
        document.getElementById('profile-title').innerText = `Hello, ${user.name} (${user.role})`;
    })
    .catch(err => console.error("Error fetching profile:", err));
}

// 2. עדכון פרטים אישיים
function updateMyProfile(event) {
    event.preventDefault();
    const originalId = document.getElementById('my-id').value;
    
    const updatedUser = {
        Id: parseInt(originalId),
        Name: document.getElementById('my-name').value,
        Password: document.getElementById('my-password').value || "", 
        Role: localStorage.getItem("userRole")
    };

    fetch(`${userUri}/${originalId}`, {
        method: 'PUT',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(updatedUser)
    })
    .then(res => {
        if (!res.ok) throw new Error("Update failed");
        showToast("Profile updated successfully!");
        localStorage.setItem("userName", updatedUser.Name);
        document.getElementById('profile-title').innerText = `Hello, ${updatedUser.Name}`;
        if (isAdmin()) getAllUsers();
    })
    .catch(err => showToast("Error: Unauthorized or invalid data"));
}

// --- פונקציה חדשה: הוספת משתמש על ידי מנהל ---
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
        document.getElementById('add-user-form').reset(); // איפוס הטופס
        getAllUsers(); // רענון הטבלה
    })
    .catch(err => showToast(err.message));
}

// 3. קבלת כל המשתמשים (רק לאדמין)
function getAllUsers() {
    const currentUserId = document.getElementById('my-id').value;

    fetch(userUri, { 
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(res => res.json())
    .then(data => {
        const tBody = document.getElementById('user-list');
        if(!tBody) return;
        
        tBody.innerHTML = '';
        data.forEach(u => {
            let tr = tBody.insertRow();
            tr.innerHTML = `
                <td>${u.id}</td>
                <td>${u.name}</td>
                <td><span class="badge">${u.role}</span></td>
                <td>
                    ${u.id == currentUserId 
                        ? '<strong>You</strong>' 
                        : `<button class="btn-delete" onclick="deleteUser(${u.id})">Delete</button>`}
                </td>
            `;
        });
    });
}

function deleteUser(id) {
    if (!confirm("Are you sure you want to delete this user?")) return;

    fetch(`${userUri}/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
    })
    .then(res => {
        if(res.ok) {
            showToast("User removed");
            getAllUsers();
        } else {
            showToast("Failed to delete user");
        }
    });
}

function logout() {
    localStorage.clear();
    window.location.href = 'login.html';
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



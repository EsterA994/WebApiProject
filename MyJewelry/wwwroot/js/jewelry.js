const uri = '/Jewelry';
const token = localStorage.getItem("token");

if (!token && !window.location.href.includes('login.html')) {
    window.location.href = 'html/login.html';
}

function showToast(msg) {
    const t = document.getElementById('toast');
    t.innerText = msg;
    t.classList.remove('hidden');
    setTimeout(() => t.classList.add('hidden'), 3000);
}

function logout() {
    localStorage.clear();
    window.location.href = 'html/login.html';
}

function getItems() {
    fetch(uri, { headers: { 'Authorization': `Bearer ${token}` }})
    .then(res => res.json())
    .then(data => {
        displayItems(data);
        if(localStorage.getItem("userRole") === "Admin") 
            document.querySelectorAll('.admin-only').forEach(el => el.classList.remove('hidden'));
    });
}

function displayItems(data) {
    const tBody = document.getElementById('jewelry');
    tBody.innerHTML = '';
    document.getElementById('counter').innerText = `You have ${data.length} items in your collection`;
    data.forEach(item => {
        let tr = tBody.insertRow();
        tr.innerHTML = `<td>${item.name}</td><td>${item.category}</td><td>${item.type}</td><td>${item.price}</td>
            <td><button class="btn-edit" onclick="displayEditForm(${item.id})">Edit</button>
            <button class="btn-delete" onclick="deleteItem(${item.id})">Delete</button></td>`;
    });
}

function addItem() {
    const item = {
        name: document.getElementById('add-name').value,
        category: document.getElementById('add-category').value,
        type: document.getElementById('add-type').value,
        price: parseInt(document.getElementById('add-price').value)
    };
    fetch(uri, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
        body: JSON.stringify(item)
    }).then(() => { showToast("Added successfully!"); getItems(); });
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${token}` }})
    .then(() => { showToast("Deleted!"); getItems(); });
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value;
    const item = {
        id: parseInt(itemId),
        name: document.getElementById('edit-name').value,
        category: document.getElementById('edit-category').value,
        type: document.getElementById('edit-type').value,
        price: parseInt(document.getElementById('edit-price').value)
    };

    fetch(`${uri}/${itemId}`, {
        method: 'PUT',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
    .then(res => {
        if (!res.ok) throw new Error("Update failed");
        showToast("Updated successfully!");
        closeInput();
        getItems();
    })
    .catch(err => showToast("Error updating item"));
}

function displayEditForm(id) {
    fetch(`${uri}/${id}`, { headers: { 'Authorization': `Bearer ${token}` }})
    .then(res => res.json())
    .then(item => {
        document.getElementById('edit-id').value = item.id;
        document.getElementById('edit-name').value = item.name;
        document.getElementById('editForm').classList.remove('hidden');
    });
}

function closeInput() { document.getElementById('editForm').classList.add('hidden'); }

// const uri = '/Jewelry';
// const token = localStorage.getItem("token");

// if (!token && !window.location.href.includes('login.html')) {
//     window.location.href = 'html/login.html';
// }

// function showToast(msg) {
//     const t = document.getElementById('toast');
//     t.innerText = msg;
//     t.classList.remove('hidden');
//     setTimeout(() => t.classList.add('hidden'), 3000);
// }

// function logout() {
//     localStorage.clear();
//     window.location.href = 'html/login.html';
// }

// function getItems() {
//     fetch(uri, { headers: { 'Authorization': `Bearer ${token}` }})
//     .then(res => res.json())
//     .then(data => {
//         displayItems(data);
//         if(localStorage.getItem("userRole") === "Admin") 
//             document.querySelectorAll('.admin-only').forEach(el => el.classList.remove('hidden'));
//     });
// }

// function displayItems(data) {
//     const tBody = document.getElementById('jewelry');
//     tBody.innerHTML = '';
//     document.getElementById('counter').innerText = `You have ${data.length} items in your collection`;
//     data.forEach(item => {
//         let tr = tBody.insertRow();
//         tr.innerHTML = `<td>${item.name}</td><td>${item.category}</td><td>${item.type}</td><td>${item.price}</td>
//             <td><button class="btn-edit" onclick="displayEditForm(${item.id})">Edit</button>
//             <button class="btn-delete" onclick="deleteItem(${item.id})">Delete</button></td>`;
//     });
// }

// function addItem() {
//     const item = {
//         name: document.getElementById('add-name').value,
//         category: document.getElementById('add-category').value,
//         type: document.getElementById('add-type').value,
//         price: parseInt(document.getElementById('add-price').value)
//     };
//     fetch(uri, {
//         method: 'POST',
//         headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
//         body: JSON.stringify(item)
//     }).then(() => { showToast("Added successfully!"); getItems(); });
// }

// function deleteItem(id) {
//     fetch(`${uri}/${id}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${token}` }})
//     .then(() => { showToast("Deleted!"); getItems(); });
// }

// function updateItem() {
//     const itemId = document.getElementById('edit-id').value;
//     const item = {
//         id: parseInt(itemId),
//         name: document.getElementById('edit-name').value,
//         category: document.getElementById('edit-category').value,
//         type: document.getElementById('edit-type').value,
//         price: parseInt(document.getElementById('edit-price').value)
//     };

//     fetch(`${uri}/${itemId}`, {
//         method: 'PUT',
//         headers: {
//             'Authorization': `Bearer ${token}`,
//             'Content-Type': 'application/json'
//         },
//         body: JSON.stringify(item)
//     })
//     .then(res => {
//         if (!res.ok) throw new Error("Update failed");
//         showToast("Updated successfully!");
//         closeInput();
//         getItems();
//     })
//     .catch(err => showToast("Error updating item"));
// }

// function displayEditForm(id) {
//     fetch(`${uri}/${id}`, { headers: { 'Authorization': `Bearer ${token}` }})
//     .then(res => res.json())
//     .then(item => {
//         document.getElementById('edit-id').value = item.id;
//         document.getElementById('edit-name').value = item.name;
//         document.getElementById('editForm').classList.remove('hidden');
//     });
// }

// function closeInput() { document.getElementById('editForm').classList.add('hidden'); }

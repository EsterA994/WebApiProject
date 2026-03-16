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


const uri = '/Jewelry';
let jewelries = [];


function getItems() {
    const token = localStorage.getItem("token");

    if (!token) {
        // ה- / בהתחלה אומר לדפדפן להתחיל משורש האתר
        window.location.href = '/html/login.html';
        return;
    }

    fetch('/Jewelry', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json'
        }
    })
        .then(response => {
            if (response.status === 401) {
                window.location.href = '/html/login.html';
                return;
            }
            return response.json();
        })
        .then(data => data && displayItems(data))
        .catch(error => console.error('Error:', error));
}


getItems();



function addItem() {
    const addNameTextbox = document.getElementById('add-name');

    const item = {
        name: addNameTextbox.value.trim(),
        category: '',
        type: '',
        price: 0,
    };

    console.log(item);

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}` // הוספת שורה זו
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(() => {
            getItems();
            addNameTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem("token")}` // חובה להוסיף!
        }
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
    const item = jewelries.find(item => item.id === id);

    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-category').value = item.category;
    document.getElementById('edit-type').value = item.type;
    document.getElementById('edit-price').value = item.price;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const itemId = document.getElementById('edit-id').value.trim();
    console.log(itemId);
    const item = {
        id: parseInt(itemId, 10),
        name: document.getElementById('edit-name').value.trim(),
        category: document.getElementById('edit-category').value.trim(),
        type: document.getElementById('edit-type').value.trim(),
        price: document.getElementById('edit-price').value.trim(),
    };


    fetch(`${uri}/${itemId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem("token")}` // חובה להוסיף!
        },
        body: JSON.stringify(item)
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to update item.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function displayCount(itemCount) {
    const name = (itemCount === 1) ? 'jewelry:' : 'jewelry kinds:';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}


function displayItems(data) {
    const tBody = document.getElementById('jewelry');
    tBody.innerHTML = '';

    displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let tr = tBody.insertRow();


        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(item.name);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNode2 = document.createTextNode(item.category);
        td2.appendChild(textNode2);

        let td3 = tr.insertCell(2);
        let textNode3 = document.createTextNode(item.type);
        td3.appendChild(textNode3);

        let td4 = tr.insertCell(3);
        let textNode4 = document.createTextNode(item.price);
        td4.appendChild(textNode4);

        let td5 = tr.insertCell(4);
        td5.appendChild(editButton);

        let td6 = tr.insertCell(5);
        td6.appendChild(deleteButton);
    });

    jewelries = data;


}
function initSignalR() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/activityHub", { accessTokenFactory: () => authToken })
        .build();

    connection.on("ReceiveActivity", function (username, action, jewelryName) {
        const activityList = document.getElementById("activityList");
        const li = document.createElement("li");
        li.textContent = `${username} ${action} '${jewelryName}'`;
        activityList.insertBefore(li, activityList.firstChild);

        // Keep only last 10 activities
        while (activityList.children.length > 10) {
            activityList.removeChild(activityList.lastChild);
        }
    });

    connection.start()
        .then(() => console.log("SignalR connected"))
        .catch(err => console.error("SignalR connection error:", err));
}
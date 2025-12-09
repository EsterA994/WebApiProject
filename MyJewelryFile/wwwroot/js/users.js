const uri = '/User';
let users = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const addNameTextbox = document.getElementById('add-name');

    const user = {
        name: addNameTextbox.value.trim(),
        age: 0,
        gender: '',
    };


    fetch(uri, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        })
        .then(response => response.json())
        .then(() => {
            getItems();
            addNameTextbox.value = '';
        })
        .catch(error => console.error('Unable to add user.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
            method: 'DELETE'
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete user.', error));
}

function displayEditForm(id) {
    const user = users.find(u => u.id === id);

    document.getElementById('edit-id').value = user.id;
    document.getElementById('edit-name').value = user.name;
    document.getElementById('edit-age').value = user.age;
    document.getElementById('edit-gender').value = user.gender;
    document.getElementById('editForm').style.display = 'block';
}

function updateItem() {
    const userId = document.getElementById('edit-id').value.trim();
    console.log(userId);
    const user = {
        id: parseInt(userId, 10),
        name: document.getElementById('edit-name').value.trim(),
        age: document.getElementById('edit-age').value.trim(),
        gender: document.getElementById('edit-gender').value.trim(),
    };


    fetch(`${uri}/${userId}`, {
            method: 'PUT',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        })
        .then(() => getItems())
        .catch(error => console.error('Unable to update user.', error));

    closeInput();

    return false;
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

        console.log(user+"----");
        

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
        let textNode3 = document.createTextNode(user.age);
        td3.appendChild(textNode3);

        let td4 = tr.insertCell(3);
        let textNode4 = document.createTextNode(user.gender);
        td4.appendChild(textNode4);

        let td5 = tr.insertCell(4);
        td5.appendChild(editButton);

        let td6 = tr.insertCell(5);
        td6.appendChild(deleteButton);
    });

    users = data;
}
const form = document.getElementById('form');
const firstName = document.getElementById('first-name');
const lastName = document.getElementById('last-name');
const email = document.getElementById('email');
const password = document.getElementById('password');


form.addEventListener('submit', e => {
    e.preventDefault();

    checkInputs();
});


function checkInputs() {
    const firstNameValue = firstName.value.trim();
    const lastNameValue = lastName.value.trim();
    const emailValue = email.value.trim();
    const passwordValue = password.value.trim();

    if(firstNameValue == '') {
        setWarningFor(firstName, 'No change made to first name.');
    }
    else {
        setSuccessFor(firstName, 'First name changed.');
    }

    if(lastNameValue == '') {
        setWarningFor(lastName, 'No change made to last name.');
    }
    else {
        setSuccessFor(lastName, 'Last name changed.');
    }

    if(emailValue == '') {
        setWarningFor(email, 'No change made to email.');
    } else if (!isEmail(emailValue)) {
        setErrorFor(email, 'Invalid email.')
    } else {
        setSuccessFor(email, 'Email changed.');
    }

    if(passwordValue == '') {
        setWarningFor(password, 'No change made to password.');
    } else {
        setSuccessFor(password, 'Password changed.');
    }
}

function setErrorFor(input, message) {
    const formControl = input.parentElement;
    const small = formControl.querySelector('small');
    small.innerText = message;
    formControl.className = 'form-control error';
}

function setWarningFor(input, message) {
    const formControl = input.parentElement;
    const small = formControl.querySelector('small');
    small.innerText = message;
    formControl.className = 'form-control warning'
}

function setSuccessFor(input, message) {
    const formControl = input.parentElement;
    const small = formControl.querySelector('small');
    small.innerText = message;
    formControl.className = 'form-control success';
}

function isEmail(email) {
    return /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(email);
}


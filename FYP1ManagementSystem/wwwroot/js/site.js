// === TOGGLE PASSWORD VISIBILITY ===
function togglePasswordVisibility(id) {
    const input = document.getElementById(id);
    if (input && input.type === "password") {
        input.type = "text";
    } else if (input) {
        input.type = "password";
    }
}

// === CONFIRM DELETE OR SUBMIT ACTION ===
function confirmAction(message, formId) {
    if (confirm(message)) {
        document.getElementById(formId).submit();
    }
}

// === AUTO-DISMISS ALERTS (IF YOU USE THEM) ===
setTimeout(function () {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => alert.style.display = 'none');
}, 4000);

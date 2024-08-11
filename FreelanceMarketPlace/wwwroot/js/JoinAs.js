document.addEventListener('DOMContentLoaded', function () {
    // Function to handle card clicks
    console.log("HELLO");
    function handleCardClick(event) {
        // Remove 'active' class from all option cards
        document.querySelectorAll('.option-card').forEach(function (card) {
            card.classList.remove('active');
        });

        // Add 'active' class to the clicked card
        var clickedCard = event.currentTarget;
        clickedCard.classList.add('active');

        // Set the corresponding radio button as checked
        clickedCard.querySelector('input[type="radio"]').checked = true;

        // Update button text based on selected role
        updateButtonText();
    }

    // Function to update button text and enable/disable it
    function updateButtonText() {
        var selectedRole = document.querySelector('input[name="role"]:checked');
        var button = document.getElementById('applyButton');
        var buttonText = selectedRole ? (selectedRole.value === 'client' ? 'Apply as a Client' : 'Apply as a Freelancer') : 'Please select an option';
        
        button.innerHTML = buttonText;
        button.disabled = !selectedRole; // Disable the button if no radio button is selected
    }

    // Add event listeners to all option cards
    document.querySelectorAll('.option-card').forEach(function (card) {
        card.addEventListener('click', handleCardClick);
    });

    // Set the default state
    document.getElementById('freelancerOption').classList.add('active');
    updateButtonText(); // Update the button text and state on page load
});
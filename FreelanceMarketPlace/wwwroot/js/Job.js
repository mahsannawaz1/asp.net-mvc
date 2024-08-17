document.addEventListener('DOMContentLoaded', () => {
    const skillButton = document.querySelector(".addSkillButton");
    const input = document.getElementById("skillInput");
    const errorDiv = document.querySelector(".emptySkill");
    const skillList = document.getElementById("skillList");
    const skillsInput = document.getElementById("skillsInput");

    skillButton.addEventListener("click", function (e) {
        e.preventDefault();
        const value = input.value.trim();
        if (value == "") {
            errorDiv.textContent = `Skill field cannot be empty.`;
        } else {
            errorDiv.textContent = ``;
            if (skillList.childElementCount == 10) {
                errorDiv.textContent = `You can't add more skills.`;
            } else {
                const newSkill = document.createElement('span');
                newSkill.className = 'badge badge-pill badge-primary mb-2 skill';
                newSkill.textContent = value;

                const removeIcon = document.createElement('i');
                removeIcon.className = 'fa-solid fa-xmark close-icon fa-md';
                newSkill.appendChild(removeIcon);

                removeIcon.addEventListener('click', function () {
                    skillList.removeChild(newSkill);
                    updateSkillsInput(); // Update hidden field on removal
                });

                skillList.appendChild(newSkill);
                updateSkillsInput(); // Update hidden field after adding
            }
        }
    });

    function updateSkillsInput() {
        const skills = Array.from(skillList.children).map(span => span.textContent.trim());
        skillsInput.value = JSON.stringify(skills); // Store skills as a JSON string
    }
});

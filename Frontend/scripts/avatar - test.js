// Vars
let domDivRockets, domRockets, domNext, domInputTeamName, domTitleTeam;
const letters = ["A", "B"];
let teamAvatars = [],
  teamNames = [],
  team = 0,
  teamNowAvatarId,
  maxTeams = 2;
const checkIfMaxTeamsIsReady = function() {
  if (team == maxTeams) {
    // Send the teams to the loadingsScreen
    window.location.href = "loadscreen.html";
  }
};
const next = function() {
  // Check if the team has selected an avatar
  if (teamNowAvatarId != null) {
    // Check if the team has choosen an name
    const teamName = domInputTeamName.value;
    if (teamName != "") {
      // Save it in the local storage
      localStorage.setItem(`teamAvatar${letters[team]}`, teamNowAvatarId);
      localStorage.setItem(`teamnaam${letters[team]}`, teamName);
      // Reset all the fields
      teamNowAvatarId = null;
      domInputTeamName.value = "";
      team++;
      domTitleTeam.innerHTML = `Teamnaam groep ${team + 1}`;
      // Check if all teams have an avatar
      checkIfMaxTeamsIsReady();
    } else {
      alert("Je moet een teamnaam invullen");
    }
  } else {
    alert("Je moet een raket kiezen");
  }
};
const clearSelectedRocket = function() {
  for (const domRocket of domRockets) {
    // Check if the team already has selected an avatar
    if (domRocket.getAttribute("data-avatarSelected") == team + 1) {
      domRocket.setAttribute("data-avatarSelected", 0);
    }
  }
};
const rocketSelected = function(domRocket) {
  // Check if the element already is taken
  if (domRocket.getAttribute("data-avatarSelected") == 0) {
    // Check if the team has already selected an avatar
    if (teamNowAvatarId != null) {
      clearSelectedRocket();
    }
    // Get the avatarId
    teamNowAvatarId = domRocket.getAttribute("data-avatarId");
    domRocket.setAttribute("data-avatarSelected", team + 1);
  }
};
const rocketsAddEventlistener = function() {
  domRockets = document.querySelectorAll(".js-rockets");
  for (const domRocket of domRockets) {
    domRocket.addEventListener("click", function() {
      rocketSelected(domRocket);
    });
  }
};
const loadAvatars = function(data) {
  // Set all the rockets into the DOM
  let output = "";
  for (const rocket of data) {
    output += `<div class="gallery__item gallery__item js-rockets" data-avatarId="${rocket["avatar_id"]}" data-avatarSelected="0" value="raketdonkerroodroze.svg">
      <img src="https://aikovanryssel.github.io/project2IMG/img/raket/svg/${rocket["name"]}.svg"
              class="gallery__img" value="raketdonkerroodroze.svg" alt="Avatar ${rocket["name"]}">
</div>`;
  }
  domDivRockets.innerHTML = output;
  // Set an eventlistener on the rockets
  rocketsAddEventlistener();
};
const loadDOM = function() {
  domDivRockets = document.querySelector(".js-div--rockets");
  domNext = document.querySelector(".js-volgende");
  domInputTeamName = document.querySelector(".js-teamname");
  domTitleTeam = document.querySelector(".js-titleTeam");
  // Check if someone clicked on the btn
  domNext.addEventListener("click", function() {
    next();
  });
};
document.addEventListener("DOMContentLoaded", function() {
  console.log("Geladen😎");
  // Load the dom-elements
  loadDOM();
  // Load the avatars
  getAPI("avatars", loadAvatars);
});

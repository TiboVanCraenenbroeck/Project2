let domUsers;
let cookieId;
const loadUsers = function(data) {
  console.log(data);
  let outputHTML = "";
  for (const user of data) {
    outputHTML += `<div><span>${user["surname"]}</span><span>${user["name"]}</span><span>${user["mail"]}</span></div>`;
  }
  domUsers.innerHTML = outputHTML;
};
const loadDom = function() {
  domUsers = document.querySelector(".js-users");
  // Load all the users form the API
  getAPI(`users?cookie_id=${cookieId}`, loadUsers);
};
document.addEventListener("DOMContentLoaded", function() {
  console.log("Script leerkrachten geladen😛");
  // Check if the user is logged in
  //cookieId = getCookie("id");
  cookieId =
    "mD8n3TZ6ib2ygIluJPpSBqfTMEap8pjds3kJfAGXGtzN6I2uiAAzO0ep0Xop3Erxi5wKjrntwvhOGe0hpTP6vfC/BL9F5Oxr9e8btDtzkpgkk3zg9yywlLpbmj82p/qDGIlYsMMOqsv7oz0hig06xw==";
  if (cookieId != null) {
    // Load the DOM
    loadDom();
  } else {
    // Send the user to the main-page
    window.location.href = "./";
  }
});

// Vars
const homaPage = "https://google.com",
  chars = ["A", "B", "C", "D"];
let gameId,
  quizId,
  urlGetQuestion = "gamevalidation/",
  countGames = 0;
// vars from the game
let playingTeam, playingQuestion;
// Vars from dom
let domTeamnamePlayingTeam, domQuestion, domAnswers;

// Function
// Fucntion for display the playing team
const displayPlayingTeam = function(dataPlayingTeam) {
  playingTeam = dataPlayingTeam;
  domTeamnamePlayingTeam.innerHTML = dataPlayingTeam["name"];
};
// Fucntion for display the new question
const displayQuestion = function(dataQuestion) {
  console.log(dataQuestion);
  playingQuestion = dataQuestion;
  // Change the title
  domQuestion.innerHTML = dataQuestion["question"];
  // Change the answer-buttons
  for (const [index, answer] of dataQuestion["answers"].entries()) {
    domAnswers[index].innerHTML = `${chars[index]}: ${answer["answer"]}`;
    // Set the id in the DOM
    domAnswers[index].setAttribute("data-answerId", answer["answer_id"]);
  }
};
// Fucntions that handles the response from the API (GameValidation)
const proccesGameValidation = function(data) {
  // Check if the gameStatug is 1
  if (data["game_status"] == 1) {
    // Display the playing team
    displayPlayingTeam(data["team"]);
    // Display the question
    displayQuestion(data["question"]);
  } else {
    // Send the user to the winningScreen
  }
};
const firstGame = function() {
  // Get the gameId and the quizId from the local storage
  try {
    quizId = localStorage.getItem("quizid");
    gameId = localStorage.getItem("gameid");
  } catch (error) {
    console.log(error);
    quizId = null;
    gameId = null;
  }
  // Check if the quizId and the gameId aren't null
  if (quizId != null && gameId != null) {
    // Get the first question from the API
    urlGetQuestion += `${quizId}/${gameId}`;
    getAPI(urlGetQuestion, proccesGameValidation);
  } else {
    console.log("Geen localstorage gevonden");
    // Send the user to the homepage
    window.location.href = homaPage;
  }
};
const loadDomElements = function() {
  domTeamnamePlayingTeam = document.querySelector(".js-teamName--playing");
  domQuestion = document.querySelector(".js-question");
  domAnswers = document.querySelectorAll(".js-answer");
};
// Load the DOM
document.addEventListener("DOMContentLoaded", function() {
  console.log("Spelen maar😎😎😎");
  // TIJDELIJK ZET GAMEID IN LOCALSTORAGE
  localStorage.setItem("quizid", "BEF11CA2-3FB0-4BDF-90D2-2AD0BE4787E6");
  localStorage.setItem("gameid", "793CE34C-E715-4955-8371-D6494331C5A1");
  // Load DOM-elements
  loadDomElements();
  // Load the first game (data)
  firstGame();
});

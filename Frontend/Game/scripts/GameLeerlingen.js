// Vars
const homaPage = "https://google.com",
  chars = ["A", "B", "C", "D"];
let gameId,
  quizId,
  urlGetQuestion = "gamevalidation/",
  countGames = 0;
// vars from the game
let playingTeam,
  playingQuestion,
  selectedAnswerId,
  numberOfCorrectAttemps,
  questionStart,
  questionStop,
  questionDuration,
  btnIdAnswerSelected;
// Vars from dom
let domTeamnamePlayingTeam, domQuestion, domAnswers;

// Functions
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
    domAnswers[index].innerHTML = `<b>${chars[index]}</b>: ${answer["answer"]}`;
    // Set the id in the DOM
    domAnswers[index].setAttribute("data-answerId", answer["answer_id"]);
  }
};
// Fucntions that handles the response from the API (GameValidation)
const proccesGameValidation = function(data) {
  // check if it is the first game
  if (countGames > 0) {
    // Clear all the answerStyles
    clearAnswers();
  }
  // Check if the gameStatus is 1
  if (data["game_status"] == 1) {
    // Display the playing team
    displayPlayingTeam(data["team"]);
    // Display the question
    displayQuestion(data["question"]);
    // Get the number of correct attemps of this team
    numberOfCorrectAttemps = data["number_of_correct_attempts"];
    // Start the timer
    questionStart = new Date().getTime();
  } else {
    // Send the user to the winningScreen
  }
  countGames++;
};
// Fucntion for the first game
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
// Function that returns ethe correct answer
const returnCorrectAnswerId = function() {
  for (const answer of playingQuestion["answers"]) {
    if (answer["correct"] == true) {
      return answer["answer_id"];
    }
  }
};
// Function that delete all the attributes
const clearAnswers = function() {
  for (const btnAnswer of domAnswers) {
    if (btnAnswer.hasAttribute("data-answerSelected")) {
      btnAnswer.removeAttribute("data-answerSelected");
    }
    // Delete the css class
    btnAnswer.classList.remove("c-answer-correct");
    btnAnswer.classList.remove("c-answer-wrong");
  }
};
// Function that show the correct answer
const showCorrectAnswer = function(jsonBody) {
  // Get the answerId
  const correctAnswerId = returnCorrectAnswerId();
  // Show the correct answer
  for (const btnAnswer of domAnswers) {
    console.log(
      btnAnswer.getAttribute("data-answerId"),
      " !!! ",
      correctAnswerId
    );
    // Check if the button has the same id
    if (btnAnswer.getAttribute("data-answerId") == correctAnswerId) {
      btnAnswer.classList.add("c-answer-correct");
    } else if (btnAnswer.hasAttribute("data-answerSelected")) {
      // Check if the it is true
      if (btnAnswer.getAttribute("data-answerSelected")) {
        btnAnswer.classList.add("c-answer-wrong");
      }
    }
  }
  // Get the new question after 5 seconds
  setTimeout(function() {
    // Convert the object to json
    const jsonApiBody = JSON.stringify(jsonBody);
    // Send the answer to the API
    getAPI(urlGetQuestion, proccesGameValidation, "POST", jsonApiBody);
  }, 5000);
};
// Function that send the user answer to the backend
const answerValidation = function() {
  // Stop the timer
  questionStop = new Date().getTime();
  // Calc the time that the users needs for answering this question in seconds
  questionDuration = Math.round((questionStop - questionStart) / 1000);
  // Make the API-body
  const apiBody = {
    game_status: 1,
    team: playingTeam,
    question: {
      question_id: playingQuestion["question_id"],
      question: playingQuestion["question"],
      difficulty: playingQuestion["difficulty"],
      answers: [
        {
          answer_id: selectedAnswerId
        }
      ]
    },
    time: questionDuration,
    number_of_correct_attempts: numberOfCorrectAttemps
  };
  showCorrectAnswer(apiBody);
};
// Function that gets the Id from the selected answer
const answerSelected = function(btnAnswer) {
  let answerIdSelected = btnAnswer.getAttribute("data-answerId");
  btnAnswer.setAttribute("data-answerSelected", "true");
  selectedAnswerId = answerIdSelected;
  answerValidation();
};
// Function that loads the DOM elements into a var
const loadDomElements = function() {
  domTeamnamePlayingTeam = document.querySelector(".js-teamName--playing");
  domQuestion = document.querySelector(".js-question");
  domAnswers = document.querySelectorAll(".js-answer");
  // Check if the users click on the button (on the screen)
  for (const btnAnswer of domAnswers) {
    btnAnswer.addEventListener("click", function() {
      btnIdAnswerSelected = btnAnswer;
      answerSelected(btnAnswer);
    });
  }
  // When the user clicked on a key
  document.addEventListener("keyup", function(e) {
    // Check wich key the user has clicked
    if ([37, 38, 39, 40].includes(e.keyCode)) {
      console.log(e.keyCode);
      domAnswers[40 - e.keyCode].click();
    }
  });
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

// Vars
const homaPage = "./",
  chars = ["A", "B", "C", "D"],
  linkImg = "https://aikovanryssel.github.io/project2IMG/";
let gameId,
  quizId,
  urlGetQuestion = "gamevalidation/",
  countGames = 0,
  root;
// vars from the game
let playingTeam,
  playingQuestion,
  selectedAnswerId,
  numberOfCorrectAttemps,
  questionStart,
  questionStop,
  questionDuration,
  btnIdAnswerSelected,
  userAnswerSelected = false,
  maxScore = 3000,
  teamSongs = {};
// Vars from dom
let domTeamnamePlayingTeam,
  domQuestion,
  domAnswers,
  domAvatarPlayingTeam,
  domRockets,
  domPlanets,
  domWinnarScreen,
  domMusic;

// Functions
// Function that switch the music
const changingSong = function(teamId) {
  console.log(teamId);
  console.log(teamSongs[teamId]);
  domMusic.src = `./music/${teamSongs[teamId]}.m4a`;
};
// Function that gets the max-score for each team
const getMaxScore = function(data) {
  maxScore = data["max_score"];
};
// Set the winning rocket
const winningRocket = function(winningRocketId) {
  domRockets[1].style.right = "111%";
  domRockets[1].src = `${linkImg}img/raket/svg/${winningRocketId}.svg`;
  setTimeout(() => {
    domRockets[1].style.bottom = "-11%";
    domRockets[1].style.width = "calc(160px*1.5)";
    // Reset the size of the planets
    /* root.style.setProperty(
      "--score--rocket-size",
      "calc(var(--score--planet-size)*1.5)"
    ); */
  }, 500);
  setTimeout(() => {
    domRockets[1].classList.add("c-winning-rocket__fly");
    domRockets[1].style.bottom = "43%";
    domRockets[1].style.right = "0.5%";
    domRockets[1].style.transform = "scale(0.3)";
  }, 1000);
  setTimeout(() => {
    window.location.href = "highscore.html";
  }, 5000);
};
// Function that clear the screen
const clearscreen = function() {
  // rockets out of window
  for (const domRocket of domRockets) {
    domRocket.style.bottom = "131%";
  }
  setTimeout(() => {
    // Drop the planets
    for (const [index, domPlanet] of domPlanets.entries()) {
      // Set the planet in the correct position
      if (index == 1) {
        domPlanet.style.bottom = "47%";
      } else {
        domPlanet.style.bottom = "-31%";
      }
    }
  }, 300);
  // Show the winner-screen
  domWinnarScreen.style.height = "100vh";
  domWinnarScreen.style.opacity = "1";
};
// function if one of the teams wins
const teamWins = function(data) {
  let teamScore = 0,
    winningTeamName,
    winningTeamAvatar;
  // Get the winning team
  for (const team of data) {
    if (team["score"] > teamScore) {
      teamScore = team["score"];
      winningTeamName = team["name"];
      winningTeamAvatar = team["avatar"]["name"];
    }
    // Check if their is a winning team
    if (teamScore == 0) {
      winningTeamName = data[0]["name"];
      winningTeamAvatar = data[0]["avatar"]["name"];
    }
  }
  clearscreen();
  setTimeout(() => {
    winningRocket(winningTeamAvatar);
  }, 300);
  // Check if their is a winning team
  if (teamScore == 0) {
    // Set the score and the name of the winning team in the DOM
    domWinnarScreen.innerHTML = `<h1>De missie is niet geslaagd!</h1`;
  } else {
    // Set the score and the name of the winning team in the DOM
    domWinnarScreen.innerHTML = `<h1>${winningTeamName}, dankzij jou is de missie geslaagd!</h1>
<h1>Punten: ${teamScore}</h1>`;
  }
};
// Change the distance of the bottom of the rocket
const changeHeightOfRocket = function(teamId, score) {
  // Clac the bottom
  const bottom = (score / maxScore) * 71;
  for (const domRocket of domRockets) {
    // Check if the element is the rocket of the team
    if (domRocket.getAttribute("data-teamId") == teamId) {
      // Set the bottom
      domRocket.style.bottom = `${bottom}%`;
    }
  }
};
// Function that gets the AvatarIds of the teams
const getAvatarsFromTeam = function(data) {
  for (const [index, domRocket] of domRockets.entries()) {
    try {
      domRocket.src = `${linkImg}img/raketrechtnieuw/svg/${data[index]["avatar"]["name"]}.svg`;
      domRocket.setAttribute("data-teamId", data[index]["team_id"]);
      // Set the music to the teams
      teamSongs[data[index]["team_id"]] = `project2song${index}`;
    } catch (error) {}
  }
};
// Fucntion for display the playing team
const displayPlayingTeam = function(dataPlayingTeam) {
  playingTeam = dataPlayingTeam;
  domTeamnamePlayingTeam.innerHTML = dataPlayingTeam["name"];
  domAvatarPlayingTeam.src = `${linkImg}img/raket/svg/${dataPlayingTeam["avatar"]["name"]}.svg`;
};
// Function for display the new question
const displayQuestion = function(dataQuestion) {
  playingQuestion = dataQuestion;
  userAnswerSelected = false;
  // Change the title
  domQuestion.innerHTML = dataQuestion["question"];
  // Change the answer-buttons
  for (const [index, answer] of dataQuestion["answers"].entries()) {
    domAnswers[index].innerHTML = `<b>${chars[index]}</b>: ${answer["answer"]}`;
    // Set the id in the DOM
    domAnswers[index].setAttribute("data-answerId", answer["answer_id"]);
  }
};
// Fucntion that handles the response from the API (GameValidation)
const proccesGameValidation = function(data) {
  // check if it is the first game
  if (countGames > 0) {
    // Clear all the answerStyles
    clearAnswers();
  }
  // Check if the gameStatus is 1
  if (data["game_status"] == 1) {
    // Set the rocket from the winning team on the correct height
    changeHeightOfRocket(data["team"]["team_id"], data["team"]["score"]);
    // Change the song for the current team
    changingSong(data["team"]["team_id"]);
    // Display the playing team
    displayPlayingTeam(data["team"]);
    // Display the question
    displayQuestion(data["question"]);
    // Get the number of correct attemps of this team
    numberOfCorrectAttemps = data["number_of_correct_attempts"];
    // Start the timer
    questionStart = new Date().getTime();
  } else if (data["game_status"] == 2) {
    // Show the winning-screen
    getAPI(`game/teams/${gameId}`, teamWins);
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
  if (quizId != null && gameId != null && quizId != "" && gameId != "") {
    // Get the avatars
    getAPI(`game/teams/${gameId}`, getAvatarsFromTeam);
    // Load the max-score of the quiz (subject)
    getAPI(`quiz/score/${quizId}`, getMaxScore);
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
    // Check if the button has the same id
    if (btnAnswer.getAttribute("data-answerId") == correctAnswerId) {
      btnAnswer.classList.add("c-answer-correct");
    } else if (btnAnswer.hasAttribute("data-answerSelected")) {
      // Check if the it is true
      if (btnAnswer.getAttribute("data-answerSelected")) {
        btnAnswer.removeAttribute("data-answerSelected");
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
  }, 3000);
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
  // Check if the user has already selected an answer
  if (!userAnswerSelected) {
    userAnswerSelected = true;
    let answerIdSelected = btnAnswer.getAttribute("data-answerId");
    btnAnswer.setAttribute("data-answerSelected", "true");
    selectedAnswerId = answerIdSelected;
    answerValidation();
  }
};
// Function that loads the DOM elements into a var
const loadDomElements = function() {
  root = document.documentElement;
  domTeamnamePlayingTeam = document.querySelector(".js-teamName--playing");
  domQuestion = document.querySelector(".js-question");
  domAnswers = document.querySelectorAll(".js-answer");
  domAvatarPlayingTeam = document.querySelector(".js-img__playing-team-avatar");
  domRockets = document.querySelectorAll(".js-rocket");
  domPlanets = document.querySelectorAll(".js-planet");
  domWinnarScreen = document.querySelector(".c-winner");
  domMusic = document.querySelector(".js-song");
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
  // Load DOM-elements
  loadDomElements();
  // Load the first game (data)
  firstGame();
});

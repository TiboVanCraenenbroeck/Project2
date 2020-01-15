let datavragen = [];
let dataantwoord = [];
let domvragen, domantwoord;
let randomnr;
let counter = 0;

const getDomElements = function() {
  console.log("dom geladen");
  let getidquiz, getidgame;
  getidquiz = localStorage.getItem("quizid");
  getidgame = localStorage.getItem("gameid");
  /*  console.log(getidquiz);
    getAPI(getidquiz); */
  getAPI("bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6");
};

const getrandomnr = function(min, max) {
  randomnr = Math.floor(Math.random() * (max - min)) + min;
};

const uitlezendata = () => {
  handleData(".json", processdata);
};

const fetchData = async function(url, method = "GET", body = null) {
  return fetch(
    `https://mctproject2.azurewebsites.net/api/v1/questions/${url}`,
    {
      method: method,
      body: body,
      headers: { "content-type": "application/json" }
    }
  )
    .then(r => r.json())
    .then(data => data);
};

let getAPI = async function(url, method = "GET", body = null) {
  try {
    const data = await fetchData(url, method, body);
    getrandomnr(0, data.length);
    /*  console.log(data.error_message) */
    for (let i = 0; i < data.length; i++) {
      let vragen = {};
      const quizvragen = data[i];
      if (quizvragen) {
        vragen["question"] = quizvragen.question;
        vragen["answers"] = quizvragen.answers;
      }
      datavragen.push(vragen);
    }

    vragenophalen();
  } catch (error) {
    console.log(error);
  }
};

const vragenophalen = function() {
  let vraag = "";
  vraag += `  <div class="js-vragen c-img__center">
        <h3 class="c-zero_margin c-color_white">${datavragen[randomnr].question}</h3>
     </div>  `;

  domvragen.innerHTML = vraag;
  let answersarr = datavragen[randomnr].answers;
  for (let i = 0; i < answersarr.length; i++) {
    const quizantwoord = answersarr[i];

    let antwoord = {};
    if (quizantwoord) {
      antwoord["answer"] = quizantwoord.answer;
      antwoord["correct"] = quizantwoord.correct;
    }
    dataantwoord.push(antwoord);
  }

  let antwoord = "";

  antwoord += `<div id="card-a" class="c-dashboard__item c-card__antwoorden  u-grid-x-1 u-grid-y-1">
    <div class="c-card__body">
        <h4 class="c-img__center">A: ${dataantwoord[0].answer}</h4>
    </div>
</div>
<div id="card-b" class="c-dashboard__item c-card__antwoorden  u-grid-x-1 u-grid-y-1">
    <div class="c-card__body">
        <h4 class="c-img__center">B:  ${dataantwoord[1].answer}</h4>
    </div>
</div>
<div id="card-c" class="c-card-c c-dashboard__item c-card__antwoorden  u-grid-x-1 u-grid-y-1">
    <div class="c-card__body">
        <h4 class="c-img__center">C:  ${dataantwoord[2].answer}</h4>                    
    </div>
</div>
<div id="card-d" class="c-card-d c-dashboard__item c-card__antwoorden  u-grid-x-1 u-grid-y-1">
    <div class="c-card__body">
        <h4 class="c-img__center">D:  ${dataantwoord[3].answer}</h4>
    </div>
</div>`;

  domantwoord.innerHTML = antwoord;
};

const changenames = function() {
  document.addEventListener("keyup", function(key) {
    counter++;
    if (counter == 1) {
      let teamnamea = "";
    }
  });
};

//makeymakey code + antwoord controle goed of fout.
const makeymakey = () => {
  document.addEventListener("keyup", function(key) {
    let diva = document.getElementById("card-a");
    let divb = document.getElementById("card-b");
    let divc = document.getElementById("card-c");
    let divd = document.getElementById("card-d");

    //keyup omdat je geen probleem zou hebben met het lan inihouden van een bepaalde toets
    switch (key.keyCode) {
      case 37:
        console.log("left");
        console.log(dataantwoord[0].correct);
        if (dataantwoord[0].correct == true) {
          diva.style.backgroundColor = "green";
        } else if (dataantwoord[1].correct == true) {
          divb.style.border = "thick solid green";
        } else if (dataantwoord[2].correct == true) {
          divc.style.border = "thick solid green";
        } else if (dataantwoord[3].correct == true) {
          divd.style.border = "thick solid green";
        }
        if (dataantwoord[0].correct == false) {
          diva.style.backgroundColor = "red";
        }
        break;
      case 38:
        console.log("up");
        if (dataantwoord[1].correct == true) {
          divb.style.backgroundColor = "green";
        } else if (dataantwoord[0].correct == true) {
          diva.style.border = "thick solid green";
        } else if (dataantwoord[2].correct == true) {
          divc.style.border = "thick solid green";
        } else if (dataantwoord[3].correct == true) {
          divd.style.border = "thick solid green";
        }
        if (dataantwoord[1].correct == false) {
          divb.style.backgroundColor = "red";
        }
        break;
      case 39:
        console.log("right");
        if (dataantwoord[2].correct == true) {
          divc.style.backgroundColor = "green";
        } else if (dataantwoord[0].correct == true) {
          diva.style.border = "thick solid green";
        } else if (dataantwoord[1].correct == true) {
          divb.style.border = "thick solid green";
        } else if (dataantwoord[3].correct == true) {
          divd.style.border = "thick solid green";
        }
        if (dataantwoord[2].correct == false) {
          divc.style.backgroundColor = "red";
        }
        break;
      case 40:
        console.log("down");
        if (dataantwoord[3].correct == true) {
          divd.style.backgroundColor = "green";
        } else if (dataantwoord[0].correct == true) {
          diva.style.border = "thick solid green";
        } else if (dataantwoord[1].correct == true) {
          divb.style.border = "thick solid green";
        } else if (dataantwoord[2].correct == true) {
          divc.style.border = "thick solid green";
        }
        if (dataantwoord[3].correct == false) {
          divd.style.backgroundColor = "red";
        }
        /*  divcorrect.style.border = 'thick solid green'; */
        break;
      case 13:
        console.log("enter");
        break;
    }
  });
};

document.addEventListener("DOMContentLoaded", function() {
  getDomElements();
  domvragen = document.querySelector(".js-vragen");
  domantwoord = document.querySelector(".js-antwoord");
  makeymakey();
});

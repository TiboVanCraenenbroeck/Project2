let id; 
let decid;
let decid2;
let uri = "https://mctproject2.azurewebsites.net/api/v1/subject?cookie_id=";
let domonderwerpen;
let domvragen;
let quiz_ids = [];
let datavragen = [];
let moeilijkheidsgraad = 0;
let selectedsubject;

const getDomElements = function(){
  console.log("dom geladen");
   // Check if the user is logged in
   if (checkIfUserIsLoggedIn()) {
    id = getCookie('id');
    decid = encodeURIComponent(id);
  } else {
    // Send the user to the main-page
    window.location.href = "./";
  }

};

function postdata(url = '', data= {})
{
  fetch(url, {
    method: 'POST', 
    body: JSON.stringify(data),
  })
  .then((response) => response.json())
  .then((data)=> {
    console.log('success:', data);
    if (data.error_message)
    {
      alert(data.error_message);
    }
    else{
      alert('vraag werd toegevoegd');
      cleanform();
    }
  })
  .catch((error)=>{
    console.error('error: ', error);
  })
}

function postonderwerpdata(url = '', data= {})
{
  fetch(url, {
    method: 'POST', 
    body: JSON.stringify(data),
  })
  .then((response) => response.json())
  .then((data)=> {
    console.log('success:', data);
    if (data.error_message)
    {
      alert(data.error_message);
    }
    else{
      alert('onderwerp werd toegevoegd');
    }
  })
  .catch((error)=>{
    console.error('error: ', error);
  })
}


const buttonclick = function(){
  let btn = document.querySelector('.js-btn-onderwerp-verzenden');
  btn.addEventListener('click', function()
  {
    var subjectinput = document.getElementById("js-input-onderwerp");
    let valuesubject = subjectinput.value
    let data = {title: valuesubject, description: ''}

    id = getCookie('id');
    decid = encodeURIComponent(id);
    postonderwerpdata(uri+decid, data)
    document.getElementById("js-input-onderwerp").value = "";  
  })
}

// home knop
const buttonhomeclick = function(){
  let btnhome = document.querySelector('.c-homebtn');
  btnhome.addEventListener('click', function(){
    setCookie('id', '');
    document.location.href = ('index.html');
  })
}

const moeilijkheid1 = function(){
  let makkelijk = document.querySelector('.js-moeilijkheid1')
  makkelijk.addEventListener('click', function(){
    makkelijk.style.color="red";
    moeilijkheidsgraad = 0;
    // terug normaal zetten andere variabelen
    let normaal = document.querySelector('.js-moeilijkheid2');
    let moeilijk = document.querySelector('.js-moeilijkheid3');
    normaal.style.color="";
    moeilijk.style.color="";

  })
}

const moeilijkheid2 = function(){
  let normaal = document.querySelector('.js-moeilijkheid2');
  normaal.addEventListener('click', function(){
    normaal.style.color="red";
    moeilijkheidsgraad = 1;
    // terug normaal zetten andere variabelen
    let makkelijk = document.querySelector('.js-moeilijkheid1');
    let moeilijk = document.querySelector('.js-moeilijkheid3');
    makkelijk.style.color="";
    moeilijk.style.color="";
  })
}

const moeilijkheid3 = function(){
  let moeilijk = document.querySelector('.js-moeilijkheid3');
  moeilijk.addEventListener('click', function(){
    moeilijk.style.color="red";
    moeilijkheidsgraad = 2;
    // terug normaal zetten andere variabelen
    let makkelijk = document.querySelector('.js-moeilijkheid1');
    let normaal = document.querySelector('.js-moeilijkheid2');
    makkelijk.style.color="";
    normaal.style.color="";
  })
}

const buttonclickvragen = function(){
  let btnvragen = document.querySelector('.js-btn-verzenden');
  btnvragen.addEventListener('click', function(){
    let vraag = document.getElementById('js-vraag').value;
    var antwoorda = document.getElementById('js-antwoorda').value;
    var antwoordb = document.getElementById('js-antwoordb').value;
    var antwoordc = document.getElementById('js-antwoordc').value;
    var antwoordd = document.getElementById('js-antwoordd').value;
    var radiobtna = document.getElementById('js-rbtn-a').checked;
    var radiobtnb = document.getElementById('js-rbtn-b').checked;
    var radiobtnc = document.getElementById('js-rbtn-c').checked;
    var radiobtnd = document.getElementById('js-rbtn-d').checked; 
    /* var level = document.getElementById('js-level').value;
    var levelnr = Number(level); */
    var levelnr = Number(moeilijkheidsgraad);

    let data = {
      question: vraag, 
      difficulty: levelnr,
      answers: [
        {
        answer: antwoorda,
        correct: radiobtna
        },
        {
        answer: antwoordb,
        correct: radiobtnb
        },
        {
        answer: antwoordc,
        correct: radiobtnc
        },
        {
        answer: antwoordd,
        correct: radiobtnd
        }
      ]  
    }
    /* console.log(data); */
    var subjectinput = document.getElementById("js-input-onderwerp-vragen");
    let valuesubject = subjectinput.value
    for (var quiz_id in quiz_ids)
    {
      if(!quiz_ids.hasOwnProperty(quiz_id)) continue;
      const everything = quiz_ids[quiz_id];
      const qid = everything.quiz_id;
      const title = everything.title;
      if (title == valuesubject)
      {
        localStorage.setItem('quizid', qid);
      }
    }
    const quizid = localStorage.getItem('quizid');
    let onderwerpid = quizid
    let apio = "https://mctproject2.azurewebsites.net/api/v1/question/"+onderwerpid+"?cookie_id=";
   /*  let apionderwerp = "https://mctproject2.azurewebsites.net/api/v1/question/bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6?cookie_id=" */
    id = getCookie('id');
    decid = encodeURIComponent(id);
    
    postdata(apio+decid,data)
    /* alert("vraag werd toegevoegd in onderwerp " + valuesubject); */
    

  })
}

const cleanform = function(){
  let vraag = document.getElementById('js-vraag').value = '';
  var antwoorda = document.getElementById('js-antwoorda').value = '';
  var antwoordb = document.getElementById('js-antwoordb').value= '';
  var antwoordd = document.getElementById('js-antwoordd').value= '';
  var antwoordc = document.getElementById('js-antwoordc').value= '';
  var radiobtna = document.getElementById('js-rbtn-a').checked= false;
  var radiobtnb = document.getElementById('js-rbtn-b').checked= false;
  var radiobtnc = document.getElementById('js-rbtn-c').checked = false;
  var radiobtnd = document.getElementById('js-rbtn-d').checked= false; 
  let makkelijk = document.querySelector('.js-moeilijkheid1').style.color = 'black';
  let normaal = document.querySelector('.js-moeilijkheid2').style.color = 'black';
  let moeilijk = document.querySelector('.js-moeilijkheid3').style.color = 'black';
  var subjectinput = document.getElementById("js-input-onderwerp-vragen").value = '';
   
  
}

//get onderwerpen

const getonderwerpen = function(){
  handleData(`https://mctproject2.azurewebsites.net/api/v1/subjects`, showonderwerpen)
}

const showonderwerpen = function(data)
{
  
  domonderwerpen = document.querySelector('.js-selecteditem');
  let arronderwerpen = data
  let OnderwerpHTML = ``;
  
  for (let i = 0; i < data.length; i++)
  {
    let ids = {};
    const quizdata = data[i];
    if(quizdata.quiz_id){
      ids['quiz_id'] = quizdata.quiz_id;
    }
    if (quizdata.title)
    {
      ids['title'] = quizdata.title;
    }
        
    quiz_ids.push(ids);

    OnderwerpHTML += `
    <datalist class="js-selecteditem" id="onderwerpen">
                  <option value="${data[i].title}">
    </datalist>
    `;
   
  }

  domonderwerpen.innerHTML = OnderwerpHTML;
}

//get vragen

const fetchData = async function(url, method = "GET", body = null) {
  return fetch(`https://mctproject2.azurewebsites.net/api/v1/questions/${url}`, {
    method: method,
    body: body,
    headers: { "content-type": "application/json" }
  })
    .then(r => r.json())
    .then(data => data);
};

let getAPI = async function(url, method = "GET", body = null) {
  datavragen = [];
    try {
      const data = await fetchData(url, method, body);
 /*      getrandomnr(0, data.length); */
     /*  console.log(data.error_message) */

      for (let i = 0; i < data.length; i++)
      {
          let vragen = {};
          const quizvragen = data[i];
          if (quizvragen)
          {
              vragen['questionid'] = quizvragen.question_id;
              vragen['question'] = quizvragen.question;
              vragen['answers'] = quizvragen.answers;
              vragen['difficult'] = quizvragen.difficulty;
          } 
          datavragen.push(vragen);
       
      }

      
    } catch (error) {
      console.log(error);
    }
  };

//delete data

function deletedata(url = '', data= {})
  {
    fetch(url, {
      method: 'delete', 
      body: JSON.stringify(data),
    })
    .then((response) => response.json())
    .then((data)=> {
      console.log('success:', data);
    })
    .catch((error)=>{
      console.error('error: ', error);
    })
  }

const buttondeletequestion = function(){
  let btn = document.querySelectorAll('.js-btn-delete');
  for (const dombtn of btn) {
    dombtn.addEventListener("click", async function(){
      const questionId = dombtn.getAttribute("data-questionId");
      console.log(questionId);
      let onderwerpid = localStorage.getItem('quizopgevraagdeid');
      id = getCookie('id');
      decid = encodeURIComponent(id);
      console.log('hier')
      let api = "https://mctproject2.azurewebsites.net/api/v1/question/"
      let preparedapi = api + onderwerpid + "/" + questionId + "?cookie_id=" + decid;
      console.log(preparedapi)
      deletedata(preparedapi);
      await alleVragen();
      
    });
  }
}
// Functie om de vragen te wijzigen
const getDataFromInputfields = function(vraagId){
  // Alle vragen ophalen
  const vraag = document.querySelector(`.js-input-question--${vraagId}`).value;
    let radiobtna = document.querySelector(`.js-radioa-question--${vraagId}`).checked;
    let radiobtnb = document.querySelector(`.js-radiob-question--${vraagId}`).checked;
    let radiobtnc = document.querySelector(`.js-radioc-question--${vraagId}`).checked;
    let radiobtnd = document.querySelector(`.js-radiod-question--${vraagId}`).checked;
    let antwoorda = document.querySelector(`.js-input-answera--${vraagId}`).value;
    let antwoordb = document.querySelector(`.js-input-answerb--${vraagId}`).value
    let antwoordc = document.querySelector(`.js-input-answerc--${vraagId}`).value
    let antwoordd = document.querySelector(`.js-input-answerd--${vraagId}`).value 
    /* var level = document.getElementById('js-level').value;
    var levelnr = Number(level); */
    // Moeilijkheidsgraad ophalen
    const levelnr = getMoeilijkheidsgraad(vraagId);

    let data = {
      question_id:vraagId,
      question: vraag, 
      difficulty: levelnr,
      answers: [
        {
        answer: antwoorda,
        correct: radiobtna
        },
        {
        answer: antwoordb,
        correct: radiobtnb
        },
        {
        answer: antwoordc,
        correct: radiobtnc
        },
        {
        answer: antwoordd,
        correct: radiobtnd
        }
      ]  
    }
    return data;
};

//put data
function putdata(url = '', data= {})
{
  fetch(url, {
    method: 'Put', 
    body: JSON.stringify(data),
  })
  .then((response) => response.json())
  .then((data)=> {
    console.log('success:', data);
    alert("vraag is gewijzigd");
  })
  .catch((error)=>{
    console.error('error: ', error);
  })
}


const btnChangeQuestion = function(){
  // Haal alle change-btns op
  const domChangeBtns= document.querySelectorAll(".js-btn--change");
  for (const domChangeBtn of domChangeBtns) {
    domChangeBtn.addEventListener("click", function(){
      const questionId = domChangeBtn.getAttribute("data-questionId");
      console.log(questionId);
      let onderwerpid = localStorage.getItem('quizopgevraagdeid');
      id = getCookie('id');
      decid = encodeURIComponent(id);
      const jsonInputFields =  getDataFromInputfields(questionId);
      console.log(jsonInputFields);
      const api = "https://mctproject2.azurewebsites.net/api/v1/question/"+onderwerpid+"?cookie_id="+decid
      putdata(api,jsonInputFields);
    });
  }
};
const getMoeilijkheidsgraad = function(vraagId){
  /*const  domBtnMoeilijkheidsgraden  = document.querySelectorAll(`.js-moeilijkheidsgraad--${vraagId}`);*/
  console.log('hier ben ik');
  const domBtnMoeilijkheidsgraad = document.querySelector(`.js-input-question--${vraagId}`).parentElement.querySelector(`.radio-input--${vraagId}:checked`).value;
  
  console.log(domBtnMoeilijkheidsgraad);
  /* return domBtnMoeilijkheidsgraad; */
};
const resetMoeilijkheidsgraadBtn = function(domBtnsMoeilijkheidsgraden){
  for (const domBtn of domBtnsMoeilijkheidsgraden) {
    domBtn.setAttribute("data-moeiljkheidsgraad", "false");
  }
};
const btnMoeilijkheidsgraad = function(){
  // Alle moeilijkheidsgraden-knoppen opvragen
  const domBtnMoeilijkheidsgraden = document.querySelectorAll(".js-moeilijkheidsgraad");
  for (const domBtn of domBtnMoeilijkheidsgraden) {
    domBtn.addEventListener("click", function(){
      // Reset
      resetMoeilijkheidsgraadBtn(domBtnMoeilijkheidsgraden);
      // Zet de attribute op true
      domBtn.setAttribute("data-moeiljkheidsgraad", "true");
    });
  }  
};
const alleVragen = async function(){
  const quizid = localStorage.getItem('quizopgevraagdeid');
      await getAPI(quizid);


      domvragen = document.querySelector('.c-form-extra');
      // let arrvragen = datavragen
      let vragenHTML = ``;
      for (let i = 0; i < datavragen.length; i++)
      {
        const quizdata = datavragen[i];
        
        vragenHTML += `<div class="c-form-delete js-form-delete">
        <input class="c-input-vragen js-input-question--${quizdata.questionid}" value="${quizdata.question}" id="js-vraag"></input>
        
        <label class="c-lbl-antwoorden c-margin">Antwoorden:</label>
    
        <input class="c-radio-a c-radiobtn-option js-radioa-question--${quizdata.questionid}" ${quizdata.answers[0].correct ? 'checked' : '' } id="js-rbtn-a${i}" type="radio" name="vragen-${i}" value="a"><br>
                
        <input class="c-radio-b c-radiobtn-option js-radiob-question--${quizdata.questionid}" ${quizdata.answers[1].correct ? 'checked' : '' } id="js-rbtn-b${i}" type="radio" name="vragen-${i}" value="b"><br>
            
        <input class="c-radio-c c-radiobtn-option js-radioc-question--${quizdata.questionid}" ${quizdata.answers[2].correct ? 'checked' : '' } id="js-rbtn-c${i}" type="radio" name="vragen-${i}" value="c"><br>
        
        <input class="c-radio-d c-radiobtn-option js-radiod-question--${quizdata.questionid}" ${quizdata.answers[3].correct ? 'checked' : '' } id="js-rbtn-d${i}" type="radio" name="vragen-${i}" value="d">  
        <input class="c-input-a c-input-style js-input-answera--${quizdata.questionid}" value="${quizdata.answers[0].answer}" id="js-antwoorda"></input>
        <input class="c-input-b c-input-style js-input-answerb--${quizdata.questionid}" value="${quizdata.answers[1].answer}" id="js-antwoordb"></input>
        <input class="c-input-c c-input-style js-input-answerc--${quizdata.questionid}" value="${quizdata.answers[2].answer}" id="js-antwoordc"></input>
        <input class="c-input-d c-input-style js-input-answerd--${quizdata.questionid}" value="${quizdata.answers[3].answer}" id="js-antwoordd"></input>
        <div class="c-div-level c-margin">
        <label class="c-lbl-level js-level--${quizdata.questionid}" data-level="${quizdata.difficult}">Moeilijkheid: </label>`;
        /* <label for="dif-1" class="c-lbl-1 js-moeilijkheid1 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">0</label>
        <label for="dif-2" class="c-lbl-2 js-moeilijkheid2 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">1</label>
        <label for="dif-3" class="c-lbl-3 js-moeilijkheid3 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">2</label>
        </div>
        <div class="c-btn-delete js-btn-delete" data-questionId="${quizdata.questionid}" id="${quizdata.questionid}"></div>
        <div class="c-btn-change js-btn--change" data-questionId="${quizdata.questionid}">wijzigen</div>
        </div> 
        `; */
        if(quizdata.difficult==0){
vragenHTML += `<label for="dif-1" class="c-lbl-1 js-moeilijkheid1 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="true">0</label>
<label for="dif-2" class="c-lbl-2 js-moeilijkheid2 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">1</label>
<label for="dif-3" class="c-lbl-3 js-moeilijkheid3 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">2</label>`;
        }else if(quizdata.difficult==1){
          vragenHTML += `<label for="dif-1" class="c-lbl-1 js-moeilijkheid1 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">0</label>
          <label for="dif-2" class="c-lbl-2 js-moeilijkheid2 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="true">1</label>
          <label for="dif-3" class="c-lbl-3 js-moeilijkheid3 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">2</label>`;
        }else{
          vragenHTML += `<label for="dif-1" class="c-lbl-1 js-moeilijkheid1 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">0</label>
          <label for="dif-2" class="c-lbl-2 js-moeilijkheid2 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="false">1</label>
          <label for="dif-3" class="c-lbl-3 js-moeilijkheid3 js-moeilijkheidsgraad--${quizdata.questionid}" data-level-selected="true">2</label>`;
        }
            vragenHTML+=`</div>
        <div class="c-btn-delete js-btn-delete" data-questionId="${quizdata.questionid}" id="${quizdata.questionid}"></div>
        <div class="c-btn-change js-btn--change" data-questionId="${quizdata.questionid}">wijzigen</div>
    </div> `;
      }

      domvragen.innerHTML = vragenHTML;
      buttondeletequestion();
      btnChangeQuestion();
      btnMoeilijkheidsgraad();
};



const buttonclickallevragen = function(){
  let btn = document.querySelector('.js-btn-vragen');
  btn.addEventListener('click', async function()
    {
      selectedsubject=document.querySelector('.js-show_subject');

      let valuesubject = selectedsubject.value
      for (var quiz_id in quiz_ids)
      {
        if(!quiz_ids.hasOwnProperty(quiz_id)) continue;
        const everything = quiz_ids[quiz_id];
        const qid = everything.quiz_id;
        const title = everything.title;
        if (title == valuesubject)
        {
          localStorage.setItem('quizopgevraagdeid', qid);
        }
      }
      alleVragen();
        })  

}


document.addEventListener('DOMContentLoaded', function()
{
    getDomElements();
    buttonclick();
    buttonclickvragen();
    buttonclickallevragen();
    moeilijkheid1();
    moeilijkheid2();
    moeilijkheid3();
    getonderwerpen();
    buttonhomeclick();
    /* getAPI("bef11ca2-3fb0-4bdf-90d2-2ad0be4787e6"); */
});
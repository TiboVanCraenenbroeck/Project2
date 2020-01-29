let dataGet;

const highscoreFilter = data => {
  dataGet = data;
  console.log(dataGet);
  if (dataGet) {
    console.log(dataGet[0]);
    console.log(dataGet[1].score);
    console.log(dataGet[2]);
  }
  let nummer2HTML = "";
  let nummer2 = document.querySelector(".gallery");
  try {
    nummer2HTML += `
      <h3 class="h01">${dataGet[1].name}</h3>
      <h3 class="h011">${dataGet[1].score}</h3>
      <h3 class="h0111">2</h3>
      <figure class="img1">
          <img src="https://aikovanryssel.github.io/project2IMG/img/raketrechtnieuw/svg/${dataGet[1].image}.svg" class="gallery__img " alt="Image 1">
      </figure>`;
  } catch (error) {}
  try {
    nummer2HTML += `<h3 class="h02">${dataGet[0].name}</h3>
      <h3 class="h022">${dataGet[0].score}</h3>
      <h3 class="h0222">1</h3>
      <figure class="img2">
              <img src="https://aikovanryssel.github.io/project2IMG/img/raketrechtnieuw/svg/${dataGet[0].image}.svg" class="gallery__img " alt="Image 1">
      </figure>`;
  } catch (error) {}
  try {
    nummer2HTML += `<h3 class="h03">${dataGet[2].name}</h3>
      <h3 class="h033">${dataGet[2].score}</h3>
      <h3 class="h0333">3</h3>
      <figure class="img3">
              <img src="https://aikovanryssel.github.io/project2IMG/img/raketrechtnieuw/svg/${dataGet[2].image}.svg" class="gallery__img" alt="Image 1">
      </figure>`;
  } catch (error) {}

  let highScoreHTML = "";
  let scores = document.querySelector(".jsdata");
  for (let i = 3; i < data.length; i++) {
    try {
      let naam = data[i].name;
      console.log(naam);
      let score = data[i].score;
      let image = data[i].image;
      console.log(score);
      highScoreHTML += `
        <div class="divTableRow">
        <div class="divTableCell01">${i + 1}</div>
                <div class="divTableCell1">
                        <figure class="imageHighscore">
                                <img src="https://aikovanryssel.github.io/project2IMG/img/raketrechtnieuw/svg/${image}.svg" class="" alt="Image 1">
                        </figure>
                </div>
                <div class="divTableCell">${naam}</div>
                <div class="divTableCell2">${score}</div>
        </div>`;
    } catch (error) {}
  }
  scores.innerHTML = highScoreHTML;
  nummer2.innerHTML = nummer2HTML;
};

const fetchData = url => {
  // Add a few headers - UITZONDERING
  let customHeaders = new Headers();
  customHeaders.append("Accept", "application/json");

  return fetch(url, { headers: customHeaders })
    .then(r => r.json())
    .then(data => data);
};
//async function
const getApi = async function(SERVER_ENDPOINT) {
  const data = await fetchData(SERVER_ENDPOINT);
  console.log(data);
  highscoreFilter(data);
};

document.addEventListener("DOMContentLoaded", function() {
  buttonhomeclick();
  let test = "https://mctproject2.azurewebsites.net/api/v1/highscores";
  getApi(test);
  domonderwerpen = document.querySelector(".js-select");
});

const buttonhomeclick = function() {
  let btnhome = document.querySelector(".js-button");
  btnhome.addEventListener("click", function() {
    console.log("click");
    window.location.href = "index.html";
  });
};

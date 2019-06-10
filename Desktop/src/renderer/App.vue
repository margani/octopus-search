<template>
  <div id="app">
    <img id="logo" src="~@/assets/logo.png" alt="electron-vue">
    <main>
      <div class="row">
        <div class="col-sm-4">
          <span class="title">Welcome to Octopus Search!</span>
          <a href="#">Search variables</a>
          <a href="#">Search projects</a>
          <a href="#">Search targets</a>
          <router-link to="/settings">Settings</router-link>
          <a href="#">About</a>
          <system-information></system-information>
        </div>
        <div class="col-sm-8">
          <router-view></router-view>
        </div>
      </div>
    </main>
  </div>
</template>

<script>
export default {
  name: "octopussearch-desktop"
};
</script>

<style>
@import "~bootstrap/dist/css/bootstrap.css";
@import "~bootstrap-vue/dist/bootstrap-vue.css";
@import url("https://fonts.googleapis.com/css?family=Source+Sans+Pro");

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: "Source Sans Pro", sans-serif;
}

#app {
  background: radial-gradient(
    ellipse at top left,
    rgba(255, 255, 255, 1) 40%,
    rgba(229, 229, 229, 0.9) 100%
  );
  height: 100vh;
  padding: 60px 80px;
  width: 100vw;
}

#logo {
  height: auto;
  margin-bottom: 20px;
  width: 420px;
}

.welcome {
  color: #555;
  font-size: 23px;
  margin-bottom: 10px;
}
/* CSS */
</style>

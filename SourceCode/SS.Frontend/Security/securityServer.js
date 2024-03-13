"use strict";
// Define the base URL for your API

const baseUrl = "http://localhost:5293/api/securityAuth";

async function postSendOTP() {
  const email = document.getElementById("emailForOTP").value;
  try {
    const response = await fetch(`${baseUrl}/postSendOTP`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: email }),
    });
    if (!response.ok) throw new Error("Failed to send OTP.");
    alert("OTP sent successfully.");
  } catch (error) {
    alert("Error sending OTP: " + error.message);
  }
}

async function postAuthenticate(event) {
  event.preventDefault(); // Prevent the form from submitting in the traditional way
  const email = document.getElementById("authEmail").value;
  const otp = document.getElementById("authOTP").value;
  try {
    const response = await fetch(`${baseUrl}/postAuthenticate`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email: email, OTP: otp }),
    });
    if (!response.ok) throw new Error("Authentication failed.");
    const principal = await response.json();
    console.log(principal);
    alert("Authentication successful.");
  } catch (error) {
    alert(error.message);
  }
}

async function postAuthorize() {
  const userId = document.getElementById("userId").value;
  const userRole = document.getElementById("userRole").value;
  const requiredPermission =
    document.getElementById("requiredPermission").value;
  const requiredResource = document.getElementById("requiredResource").value;

  const requestBody = {
    currentPrincipal: { userId: userId, role: userRole },
    requiredClaims: {
      permission: requiredPermission,
      resource: requiredResource,
    },
  };

  try {
    const response = await fetch(`${baseUrl}/postAuthorize`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(requestBody),
    });
    if (!response.ok) throw new Error("Authorization check failed.");
    alert("Authorization check passed.");
  } catch (error) {
    alert(error.message);
  }
}

// Example usage:
// sendOTP('user@example.com');
// authenticate('user@example.com', 'OTP_received');
// authorize({userId: 'user1'}, {claim1: 'Admin'});

//})(window, window.ajaxClient);

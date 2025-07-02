const apiBase = "/api";

//Start : Webcam
const video = document.getElementById("video");
const canvas = document.getElementById("canvas");
const verifyImageInput = document.getElementById("verifyImage");
const verifyPreview = document.getElementById("verifyPreview");
const verifyAlert = document.getElementById("verifyAlert");
let stream = null;

// === Webcam Functions ===
function startWebcam() {
    navigator.mediaDevices.getUserMedia({ video: true })
        .then(s => {
            stream = s;
            video.srcObject = stream;
            video.style.display = "block";
        })
        .catch(err => alert("Camera access denied."));
}

function stopWebcam() {
    if (stream) {
        stream.getTracks().forEach(track => track.stop());
        video.style.display = "none";
    }
}

function captureImage() {
    canvas.getContext("2d").drawImage(video, 0, 0, canvas.width, canvas.height);
    canvas.toBlob(blob => {
        // Convert webcam blob to File and attach to input
        const file = new File([blob], "webcam.jpg", { type: "image/jpeg" });
        const dt = new DataTransfer();
        dt.items.add(file);
        verifyImageInput.files = dt.files;

        // Preview captured image
        verifyPreview.src = URL.createObjectURL(blob);
        verifyPreview.style.display = "block";
    }, "image/jpeg");
}

//End: Webcam

document.addEventListener("DOMContentLoaded", () => {
    const registerForm = document.getElementById("registerForm");
    const verifyForm = document.getElementById("verifyForm");

    const registerImageInput = document.getElementById("registerImage");
    const verifyImageInput = document.getElementById("verifyImage");
    const registerPreview = document.getElementById("registerPreview");
    const verifyPreview = document.getElementById("verifyPreview");

    // Preview uploaded images
    function showPreview(input, preview) {
        const file = input.files[0];
        if (file) {
            preview.src = URL.createObjectURL(file);
            preview.style.display = "block";
        } else {
            preview.src = "#";
            preview.style.display = "none";
        }
    }

    if (registerImageInput) {
        registerImageInput.addEventListener("change", () =>
            showPreview(registerImageInput, registerPreview)
        );
    }

    if (verifyImageInput) {
        verifyImageInput.addEventListener("change", () =>
            showPreview(verifyImageInput, verifyPreview)
        );
    }

    if (registerForm) {
        registerForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const formData = new FormData(registerForm);
            const alertBox = document.getElementById("registerAlert");

            try {
                const res = await fetch(`${apiBase}/user/register`, {
                    method: "POST",
                    body: formData,
                });

                const result = await res.json();

                if (res.ok) {
                    alertBox.className = "alert alert-success mt-3";
                    alertBox.innerText = "User registered successfully!";
                    registerForm.reset();
                    registerPreview.style.display = "none";
                } else {
                    alertBox.className = "alert alert-danger mt-3";
                    alertBox.innerText = result.message || "Registration failed.";
                }
                alertBox.classList.remove("d-none");
            } catch (error) {
                alertBox.className = "alert alert-danger mt-3";
                alertBox.innerText = "Error: " + error;
                alertBox.classList.remove("d-none");
            }
        });
    }

    if (verifyForm) {
        verifyForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const formData = new FormData(verifyForm);
            const alertBox = document.getElementById("verifyAlert");

            try {
                const res = await fetch(`${apiBase}/verify/face`, {
                    method: "POST",
                    body: formData,
                });

                const result = await res.json();

                if (res.ok) {
                    alertBox.className = "alert alert-success mt-3";
                    alertBox.innerText = result.message || "Face verified successfully!";
                    verifyForm.reset();
                    verifyPreview.style.display = "none";
                } else {
                    alertBox.className = "alert alert-warning mt-3";
                    alertBox.innerText = result.message || "Face does not match.";
                }
                alertBox.classList.remove("d-none");
            } catch (error) {
                alertBox.className = "alert alert-danger mt-3";
                alertBox.innerText = "Error: " + error;
                alertBox.classList.remove("d-none");
            } 
        });
    }
});

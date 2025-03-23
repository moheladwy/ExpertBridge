import { useEffect, useState } from "react";
import { AddPostRequest } from "../../../../features/posts/types";
import { useCreatePostMutation } from "../../../../features/posts/postsSlice";
import toast from "react-hot-toast";
import {
  Button,
  Step,
  StepLabel,
  Stepper,
  TextField,
  Typography,
  Box,
  Modal,
  IconButton,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";

const steps = ["Enter Title", "Write Content", "Add Media"];

const CreatePostModal: React.FC = () => {
  const [createPost, { isLoading, isSuccess, isError }] =
    useCreatePostMutation();

  const [open, setOpen] = useState(false);
  const [activeStep, setActiveStep] = useState(0);
  const [title, setTitle] = useState("");
  const [body, setBody] = useState("");
  const [media, setMedia] = useState<File[]>([]);
  const [error, setError] = useState("");

  useEffect(() => {
    if (isError) toast.error("An error occurred while creating your post");
    if (isSuccess) {
      toast.success("Post created successfully");
      handleClose();
    }
  }, [isSuccess, isError]);

  const handleOpen = () => setOpen(true);
  const handleClose = () => {
    setOpen(false);
    resetForm();
  };

  const resetForm = () => {
    setTitle("");
    setBody("");
    setMedia([]);
    setActiveStep(0);
    setError("");
  };

  const handleNext = () => {
    if (activeStep === 0 && !title.trim()) {
      setError("Title is required.");
      return;
    }
    if (activeStep === 1 && !body.trim()) {
      setError("Content is required.");
      return;
    }
    setError("");
    setActiveStep((prev) => prev + 1);
  };

  const handleBack = () => setActiveStep((prev) => prev - 1);

  const handleSubmit = async () => {
    await createPost({ title, content: body });
  };

  return (
    <>
      {/* Button to Open Modal */}
      <Button variant="contained" onClick={handleOpen}>
        Create New Post
      </Button>

      {/* Modal */}
      <Modal open={open} onClose={handleClose} aria-labelledby="create-post-modal">
        <Box
          sx={{
            position: "absolute",
            top: "50%",
            left: "50%",
            transform: "translate(-50%, -50%)",
            width: 500,
            bgcolor: "white",
            boxShadow: 24,
            borderRadius: 2,
            p: 4,
          }}
        >
          {/* Close Button */}
          <IconButton
            sx={{ position: "absolute", top: 10, right: 10 }}
            onClick={handleClose}
          >
            <CloseIcon />
          </IconButton>

          <Typography variant="h6" gutterBottom id="create-post-modal">
            Create a Post
          </Typography>

          {/* Stepper */}
          <Stepper activeStep={activeStep} alternativeLabel>
            {steps.map((label) => (
              <Step key={label}>
                <StepLabel>{label}</StepLabel>
              </Step>
            ))}
          </Stepper>

          <Box sx={{ my: 3 }}>
            {error && <Typography color="error">{error}</Typography>}

            {activeStep === 0 && (
              <TextField
                fullWidth
                label="Post Title"
                variant="outlined"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
              />
            )}

            {activeStep === 1 && (
              <TextField
                fullWidth
                label="Post Content"
                variant="outlined"
                multiline
                rows={4}
                value={body}
                onChange={(e) => setBody(e.target.value)}
              />
            )}

            {activeStep === 2 && (
              <Button variant="contained" component="label">
                Upload Media
                <input type="file" hidden multiple onChange={(e) => setMedia([...media, ...(e.target.files || [])])} />
              </Button>
            )}
          </Box>

          {/* Navigation Buttons */}
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Button disabled={activeStep === 0} onClick={handleBack}>
              Back
            </Button>
            {activeStep === steps.length - 1 ? (
              <Button variant="contained" onClick={handleSubmit} disabled={isLoading}>
                Submit
              </Button>
            ) : (
              <Button variant="contained" onClick={handleNext}>
                Next
              </Button>
            )}
          </Box>
        </Box>
      </Modal>
    </>
  );
};

export default CreatePostModal;

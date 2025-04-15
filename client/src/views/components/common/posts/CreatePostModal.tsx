import { useEffect, useState } from "react";
import { AddPostRequest } from "../../../../features/posts/types";
import { useCreatePostMutation } from "../../../../features/posts/postsSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
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
import {
  Avatar
} from "@/views/components/custom/avatar"

import CloudUploadIcon from '@mui/icons-material/CloudUpload';

const steps = ["Ask Question", "Describe Your Problem", "Add Media"];

const CreatePostModal: React.FC = () => {
  const [createPost, { isLoading, isSuccess, isError }] =
    useCreatePostMutation();

  const [open, setOpen] = useState(false);
  const [activeStep, setActiveStep] = useState(0);
  const [title, setTitle] = useState("");
  const [body, setBody] = useState("");
  const [media, setMedia] = useState<File[]>([]);
  const [error, setError] = useState("");
  const [,,,authUser,userProfile] = useIsUserLoggedIn();

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
      setError("This Field is required.");
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
      <div className="flex justify-center items-center gap-2 bg-white shadow-md rounded-lg p-4 border border-gray-200" onClick={handleOpen}>
        <Avatar className="bg-white flex justify-center items-center">
          {/* using the name's first letter as a profile */}
          {
            userProfile?.profilePictureUrl
              ? <img
                src={userProfile.profilePictureUrl}
                width={40}
                height={40}
                className="rounded-full"
              />
              : <h1 className="text-main-blue font-bold text-lg ">{authUser?.displayName?.charAt(0).toUpperCase()}</h1>
          }
        </Avatar>
        <Button className=" bg-gray-100 text-gray-500 px-5 hover:bg-gray-200 hover:text-main-blue w-full rounded-full">
          <div className="w-full text-left">What do you want to ask?</div>
        </Button>
      </div>

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

          className="max-sm: w-auto"
        >
          {/* Close Button */}
          <IconButton
            sx={{ position: "absolute", top: 10, right: 10 }}
            onClick={handleClose}
          >
            <CloseIcon />
          </IconButton>

          <Typography variant="h6" gutterBottom id="create-post-modal" className="max-sm:text-md">
            Ask a question...
          </Typography>

          {/* Stepper */}
          <Stepper activeStep={activeStep} alternativeLabel>
            {steps.map((label) => (
              <Step key={label}  >
                <StepLabel className="text-">{label}</StepLabel>
              </Step>
            ))}
          </Stepper>

          <Box className="flex justify-center items-center" sx={{ my: 3 }}>
            {error && <Typography color="error">{error}</Typography>}

            {/* Title */}
            {activeStep === 0 && (
              <TextField
                fullWidth
                label="Start Asking Your Question"
                variant="outlined"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
              />
            )}

            {/* Descrebtion */}
            {activeStep === 1 && (
              <TextField
                fullWidth
                label="Describe Your Problem"
                variant="outlined"
                multiline
                rows={4}
                value={body}
                onChange={(e) => setBody(e.target.value)}
              />
            )}

            {/* media */}
            {activeStep === 2 && (
              <Button 
                component="label"
                role={undefined}
                variant="contained"
                tabIndex={-1}
                startIcon={<CloudUploadIcon />}
              >
                Upload Media
                <input type="file" hidden multiple onChange={(e) => setMedia([...media, ...(e.target.files || [])])} />
              </Button>
            )}
          </Box>

          {/* Navigation Buttons */}
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Button disabled={activeStep === 0} onClick={handleBack} className="text-red-600">
              Back
            </Button>
            {activeStep === steps.length - 1 ? (
              <Button variant="contained" onClick={handleSubmit} disabled={isLoading} className="bg-main-blue hover:bg-blue-950">
                Submit
              </Button>
            ) : (
              <Button variant="contained" onClick={handleNext} className="bg-main-blue hover:bg-blue-950">
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

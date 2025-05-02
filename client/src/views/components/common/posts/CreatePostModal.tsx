import { useCallback, useEffect, useState } from "react";
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
  FormHelperText,
} from "@mui/material";
import CloseIcon from "@mui/icons-material/Close";
import useCallbackOnMediaUploadSuccess from "@/hooks/useCallbackOnMediaUploadSuccess";
import FileUploadForm from "../../custom/FileUploadForm";
import { MediaObject } from "@/features/media/types";
import { z } from "zod";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg"
import { useAuthPrompt } from "@/contexts/AuthPromptContext";

// Character limits
const TITLE_MAX_LENGTH = 256;
const BODY_MAX_LENGTH = 5000;

// Zod schema for form validation
const postSchema = z.object({
  title: z.string()
    .min(1, "Title is required")
    .max(TITLE_MAX_LENGTH, "Title cannot exceed 256 characters")
    .refine(
      (val) => val.trim().split(/\s+/).filter(Boolean).length >= 3,
      { message: "Title must be at least 3 words" }
    ),
  content: z.string()
    .min(1, "Content is required")
    .max(BODY_MAX_LENGTH, "Content cannot exceed 5000 characters")
    .refine(
      (val) => val.trim().split(/\s+/).filter(Boolean).length >= 10,
      { message: "Content must be at least 10 words" }
    ),
});

const steps = ["Ask Question", "Describe Your Problem", "Add Media"];

const CreatePostModal: React.FC = () => {
  useEffect(() => {
    console.log('modal mounting...');
  }, []);

  const [open, setOpen] = useState(false);
  const [activeStep, setActiveStep] = useState(0);
  const [title, setTitle] = useState("");
  const [body, setBody] = useState("");
  const [media, setMedia] = useState<File[]>([]);
  const [error, setError] = useState("");
  const [titleError, setTitleError] = useState("");
  const [bodyError, setBodyError] = useState("");
  const [mediaList, setMediaList] = useState<MediaObject[]>([]);
  const [isLoggedIn, , , authUser, userProfile] = useIsUserLoggedIn();
  const { showAuthPrompt } = useAuthPrompt();

  const [createPost, createPostResult] =
    useCreatePostMutation();

  const {
    uploadResult,
    uploadMedia,
  } = useCallbackOnMediaUploadSuccess(createPost, { title, content: body });

  const {
    isSuccess,
    isError,
    isLoading,
  } = createPostResult;


  useEffect(() => {
    if (isError) toast.error("An error occurred while creating your post");
    if (isSuccess) {
      toast.success("Post created successfully");
      handleClose();
    }
  }, [isSuccess, isError]);

  const handleOpen = () => {
    if (isLoggedIn) {
      setOpen(true);
    } else {
      showAuthPrompt();
    }
  }

  const resetForm = useCallback(() => {
    setTitle("");
    setBody("");
    setMedia([]);
    setActiveStep(0);
    setError("");
    setTitleError("");
    setBodyError("");
  }, []);

  const handleClose = useCallback(() => {
    setOpen(false);
    resetForm();
  }, [setOpen, resetForm]);
  
  // Updated handleNext function with direct validation
  const handleNext = () => {
    // Validate based on current step
    if (activeStep === 0) {
      try {
        // Validate the title string directly
        postSchema.shape.title.parse(title);
        setTitleError(""); // Clear error if validation passes
        setActiveStep((prev) => prev + 1); // Proceed to next step
      } catch (err) {
        // Handle validation errors
        if (err instanceof z.ZodError) {
          setTitleError(err.errors[0].message);
        } else {
          setTitleError("An unexpected error occurred");
        }
      }
    } else if (activeStep === 1) {
      try {
        // Validate the body string directly
        postSchema.shape.content.parse(body);
        setBodyError(""); // Clear error if validation passes
        setActiveStep((prev) => prev + 1); // Proceed to next step
      } catch (err) {
        // Handle validation errors
        if (err instanceof z.ZodError) {
          setBodyError(err.errors[0].message);
        } else {
          setBodyError("An unexpected error occurred");
        }
      }
    } else {
      // If we're not on a step that needs validation, just proceed
      setActiveStep((prev) => prev + 1);
    }
  };

  const handleBack = () => setActiveStep((prev) => prev - 1);

  const handleSubmit = async () => {
    try {
      postSchema.parse({ title, content: body });
      setTitleError("");
      setBodyError("");
      await uploadMedia({ mediaList });
    } catch (err) {
      if (err instanceof z.ZodError) {
        // Set errors for specific fields
        err.errors.forEach(error => {
          if (error.path[0] === 'title') {
            setTitleError(error.message);
          } else if (error.path[0] === 'content') {
            setBodyError(error.message);
          }
        });
      }
    }
  };
  
  // Handle title input with character limit
  const handleTitleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    if (newValue.length <= TITLE_MAX_LENGTH) {
      setTitle(newValue);
      if (newValue.trim()) {
        setTitleError(""); // Clear error only if there's valid content
      }
    }
  };
  
  // Handle body input with character limit
  const handleBodyChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    if (newValue.length <= BODY_MAX_LENGTH) {
      setBody(newValue);
      if (newValue.trim()) {
        setBodyError(""); // Clear error only if there's valid content
      }
    }
  };

  const titleCharsLeft = TITLE_MAX_LENGTH - title.length;
  const bodyCharsLeft = BODY_MAX_LENGTH - body.length;

  return (
    <>
      <div className="flex justify-center items-center gap-2 bg-white shadow-md rounded-lg p-4 border border-gray-200" onClick={handleOpen}>
        {isLoggedIn && <div className="bg-white flex justify-center items-center">
          {
            userProfile?.profilePictureUrl
              ? <img
                src={userProfile.profilePictureUrl}
                width={45}
                height={45}
                className="rounded-full"
              />
              : <img 
                src={defaultProfile}
                alt="Profile Picture"
                width={45}
                height={45}
                className="rounded-full"
              />
          }
        </div>}
        <Button className=" bg-gray-100 text-gray-500 px-5 hover:bg-gray-200 hover:text-main-blue w-full rounded-full">
          <div className="w-full text-left">What do you want to ask?</div>
        </Button>
      </div>

      {/* Modal */}
      <Modal
        open={open}
        onClose={handleClose}
        aria-labelledby="create-post-modal"
        aria-disabled={isLoading || uploadResult.isLoading}
      >
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

          <Box className="flex flex-col items-center w-full" sx={{ my: 3 }}>
            {/* Title */}
            {activeStep === 0 && (
              <div className="w-full">
                <TextField
                  fullWidth
                  label="Start Asking Your Question"
                  variant="outlined"
                  value={title}
                  onChange={handleTitleChange}
                  inputProps={{
                    maxLength: TITLE_MAX_LENGTH,
                    dir: "auto"
                  }}
                  required
                  error={!!titleError}
                  helperText={titleError || ""}
                />
                
                {!titleError && (
                  <div className="flex justify-end mt-1">
                    <Typography 
                      variant="caption" 
                      color={titleCharsLeft < 1 ? "error" : "green"}
                    >
                      {titleCharsLeft} characters left
                    </Typography>
                  </div>
                )}
              </div>
            )}

            {/* Description */}
            {activeStep === 1 && (
              <div className="w-full">
                <TextField
                  fullWidth
                  label="Describe Your Problem"
                  variant="outlined"
                  multiline
                  rows={4}
                  value={body}
                  onChange={handleBodyChange}
                  inputProps={{
                    maxLength: BODY_MAX_LENGTH,
                    dir: "auto"
                  }}
                  required
                  error={!!bodyError}
                  helperText={bodyError || ""}
                />
                
                {!bodyError && (
                  <div className="flex justify-end mt-1">
                    <Typography 
                      variant="caption"
                      color={bodyCharsLeft < 1 ? "error" : "green"}
                    >
                      {bodyCharsLeft} characters left
                    </Typography>
                  </div>
                )}
              </div>
            )}

            {/* media */}
            {activeStep === 2 && (
              <FileUploadForm onSubmit={handleSubmit} setParentMediaList={setMediaList} />
            )}
          </Box>

          {/* Navigation Buttons */}
          <Box sx={{ display: "flex", justifyContent: "space-between" }}>
            <Button disabled={activeStep === 0} onClick={handleBack} className="text-red-600">
              Back
            </Button>
            {activeStep === steps.length - 1 ? (
              <Button variant="contained" onClick={handleSubmit} disabled={isLoading || uploadResult.isLoading} className="bg-main-blue hover:bg-blue-950">
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

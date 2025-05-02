import React, { useState, useEffect } from "react"; // Import useEffect
import { z } from "zod";
import { useUpdatePostMutation } from "@/features/posts/postsSlice";
import { Post } from "@/features/posts/types";
import { Button } from "@/views/components/ui/button";
import { Input } from "@/views/components/ui/input";
import { Textarea } from "@/views/components/ui/textarea";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/views/components/ui/card";
import { Loader2 } from "lucide-react";
import toast from "react-hot-toast";

const editPostSchema = z.object({
  title: z.string()
    .min(3, "Title must be at least 3 characters")
    .max(256, "Title must be less than 256 characters"),
  content: z.string()
    .min(3, "Content must be at least 3 characters")
    .max(5000, "Content must be less than 5000 characters"),
});

type EditPostFormData = z.infer<typeof editPostSchema>;

interface EditPostModalProps {
  post: Post;
  isOpen: boolean;
  onClose: () => void;
}

const EditPostModal: React.FC<EditPostModalProps> = ({ post, isOpen, onClose }) => {
  const [formData, setFormData] = useState<EditPostFormData>({
    title: post.title,
    content: post.content || "",
  });
  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  const [updatePost, { isLoading }] = useUpdatePostMutation();

  // Function to validate the form data
  const validate = (): boolean => {
    try {
      editPostSchema.parse(formData);
      setErrors({}); // Clear errors if validation passes
      return true;
    } catch (error) {
      if (error instanceof z.ZodError) {
        const newErrors: { [key: string]: string } = {};
        error.errors.forEach((err) => {
          if (err.path.length > 0) { // Check path length
            newErrors[err.path[0]] = err.message;
          }
        });
        setErrors(newErrors); // Set new errors if validation fails
      }
      return false;
    }
  };

  // Re-validate whenever formData changes
  useEffect(() => {
    validate();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [formData]); // Dependency array ensures validation runs when formData updates

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
    // No need to call validate() here anymore, useEffect handles it
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    // Re-run validation on submit just to be absolutely sure and prevent submission if invalid
    if (!validate()) {
      return;
    }

    try {
      await updatePost({
        postId: post.id,
        title: formData.title,
        content: formData.content,
      }).unwrap();
      toast.success("Post updated successfully");
      onClose();
    } catch (error) {
      toast.error("Failed to update post");
      console.error("Error updating post:", error);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50 p-4 backdrop-blur-sm">
      {/* Added backdrop-blur-sm for better background effect */}
      <Card className="w-full max-w-lg mx-auto"> {/* Increased max-width slightly */}
        <CardHeader>
          {/* Centered, larger, and bold title */}
          <CardTitle className="text-center text-2xl font-bold">
            Edit Post
          </CardTitle>
        </CardHeader>
        <form onSubmit={handleSubmit}>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <label htmlFor="title" className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
                Title
              </label>
              <Input
                id="title"
                name="title"
                value={formData.title}
                onChange={handleChange}
                disabled={isLoading}
                className={errors.title ? "border-red-500 focus-visible:ring-red-500" : ""} // Added focus style for error
                aria-invalid={!!errors.title} // Accessibility improvement
                aria-describedby={errors.title ? "title-error" : undefined} // Accessibility improvement
                dir="auto"
              />
              {errors.title && (
                <p id="title-error" dir="auto" className="text-red-500 text-sm">{errors.title}</p> // Added id for aria-describedby
              )}
            </div>
            <div className="space-y-2">
              <label htmlFor="content" className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70">
                Content
              </label>
              <Textarea
                id="content"
                name="content"
                value={formData.content}
                onChange={handleChange}
                disabled={isLoading}
                rows={6} // Slightly increased rows
                className={errors.content ? "border-red-500 focus-visible:ring-red-500" : ""} // Added focus style for error
                aria-invalid={!!errors.content} // Accessibility improvement
                aria-describedby={errors.content ? "content-error" : undefined} // Accessibility improvement
                dir="auto"
              />
              {errors.content && (
                <p id="content-error" className="text-red-500 text-sm">{errors.content}</p> // Added id for aria-describedby
              )}
            </div>
          </CardContent>
          <CardFooter className="flex justify-end space-x-3"> {/* Increased space */}
            <Button
              type="button"
              variant="destructive" // Changed variant to destructive for red styling
              onClick={onClose}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              disabled={isLoading || Object.keys(errors).length > 0} // Disability logic remains the same, but `errors` state is now reactive
            >
              {isLoading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Saving...
                </>
              ) : (
                "Save Changes"
              )}
            </Button>
          </CardFooter>
        </form>
      </Card>
    </div>
  );
};

export default EditPostModal;

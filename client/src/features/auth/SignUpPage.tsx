import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import useCreateUserWithEmailAndPassword from "@/lib/firebase/EmailAuth/useCreateUserWithEmailAndPassword";
import { auth } from "@/lib/firebase";

const SignUpPage: React.FC = () => {
  const navigate = useNavigate();
  // const [signUp, { isLoading, error }] = useSignUpMutation();
  const [
    registerWithEmailAndPassword,
    registeredUser,
    loading,
    error
  ] = useCreateUserWithEmailAndPassword(auth, { sendEmailVerification: true });

  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  useEffect(() => {
    if (registeredUser) {
      navigate("/login");
    }
  }, [registeredUser, navigate]);

  // handle errors
  useEffect(() => {
    if (error) {
      console.error('useEffecting', error);
    }
  }, [error]);

  const validate = () => {
    const newErrors: { [key: string]: string } = {};
    if (!formData.email.includes("@")) newErrors.email = "Invalid email";
    if (formData.password.length < 12)
      newErrors.password = "Password must be at least 12 characters";

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    await registerWithEmailAndPassword(formData.email, formData.password);
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="flex justify-center items-center h-screen">
      <div className="p-6 bg-white shadow-lg rounded-xl w-96">
        <h2 className="text-2xl font-bold mb-4 text-center">Sign Up</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-sm font-medium">Email</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              className="w-full p-2 border rounded"
            />
            {errors.email && <p className="text-red-500 text-sm">{errors.email}</p>}
          </div>
          <div>
            <label className="block text-sm font-medium">Password</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              className="w-full p-2 border rounded"
            />
            {errors.password && <p className="text-red-500 text-sm">{errors.password}</p>}
          </div>
          <button
            type="submit"
            className="w-full bg-blue-500 text-white p-2 rounded hover:bg-blue-600"
            disabled={loading}
          >
            {loading ? "Signing Up..." : "Sign Up"}
          </button>
          {error && <p className="text-red-500 text-sm text-center">Signup failed {error.message}</p>}
        </form>
      </div>
    </div>
  );
};

export default SignUpPage;

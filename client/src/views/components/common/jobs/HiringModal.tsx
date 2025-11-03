import { useState } from "react";
import {
	Dialog,
	DialogContent,
	DialogHeader,
	DialogTitle,
} from "@/views/components/ui/dialog";
import { Button } from "@/views/components/ui/button";
import { Input } from "@/views/components/ui/input";
import { Label } from "@/views/components/ui/label";
import { Textarea } from "@/views/components/ui/textarea";
import {
	Select,
	SelectContent,
	SelectItem,
	SelectTrigger,
	SelectValue,
} from "@/views/components/ui/select";
import {
	DollarSign,
	Briefcase,
	FileText,
	MapPin,
	Clock,
	User,
	Upload,
	X,
} from "lucide-react";
import { useCreateJobOfferMutation } from "@/features/jobs/jobsSlice";
import { CreateJobOfferRequest } from "@/features/jobs/types";
import { toast } from "sonner";
import { ProfileResponse } from "@/features/profiles/types";

interface HiringModalProps {
	isOpen: boolean;
	onClose: () => void;
	onSuccess: () => void;
	expertProfile: ProfileResponse;
}

const HiringModal = ({
	isOpen,
	onClose,
	onSuccess,
	expertProfile,
}: HiringModalProps) => {
	const [formData, setFormData] = useState({
		title: "",
		description: "",
		budget: "",
		budgetType: "fixed",
		area: "",
		timeline: "",
		attachments: [] as File[],
	});
	const [errors, setErrors] = useState<Record<string, string>>({});
	const [dragActive, setDragActive] = useState(false);

	const [createJobOffer, { isLoading }] = useCreateJobOfferMutation();

	const validateForm = () => {
		const newErrors: Record<string, string> = {};

		if (!formData.title.trim()) {
			newErrors.title = "Project title is required";
		}

		if (!formData.description.trim()) {
			newErrors.description = "Project description is required";
		}

		if (!formData.budget || parseFloat(formData.budget) <= 0) {
			newErrors.budget = "Budget must be greater than 0";
		}

		if (!formData.area.trim()) {
			newErrors.area = "Location is required";
		}

		if (!formData.timeline.trim()) {
			newErrors.timeline = "Timeline is required";
		}

		setErrors(newErrors);
		return Object.keys(newErrors).length === 0;
	};

	const handleInputChange = (field: string, value: string) => {
		setFormData((prev) => ({ ...prev, [field]: value }));
		if (errors[field]) {
			setErrors((prev) => ({ ...prev, [field]: "" }));
		}
	};

	const handleFileUpload = (files: FileList | null) => {
		if (!files) return;

		const newFiles = Array.from(files).filter((file) => {
			const isValidType =
				file.type.includes("image/") ||
				file.type.includes("application/pdf") ||
				file.type.includes("application/msword") ||
				file.type.includes("text/");
			const isValidSize = file.size <= 10 * 1024 * 1024; // 10MB limit

			if (!isValidType) {
				toast.error(`${file.name} is not a supported file type`);
				return false;
			}

			if (!isValidSize) {
				toast.error(`${file.name} is too large (max 10MB)`);
				return false;
			}

			return true;
		});

		setFormData((prev) => ({
			...prev,
			attachments: [...prev.attachments, ...newFiles].slice(0, 5), // Max 5 files
		}));
	};

	const removeFile = (index: number) => {
		setFormData((prev) => ({
			...prev,
			attachments: prev.attachments.filter((_, i) => i !== index),
		}));
	};

	const handleDrag = (e: React.DragEvent) => {
		e.preventDefault();
		e.stopPropagation();
		if (e.type === "dragenter" || e.type === "dragover") {
			setDragActive(true);
		} else if (e.type === "dragleave") {
			setDragActive(false);
		}
	};

	const handleDrop = (e: React.DragEvent) => {
		e.preventDefault();
		e.stopPropagation();
		setDragActive(false);
		handleFileUpload(e.dataTransfer.files);
	};

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();

		if (!validateForm()) return;

		const request: CreateJobOfferRequest = {
			title: formData.title,
			description: formData.description,
			budget: parseFloat(formData.budget),
			area: formData.area,
			workerId: expertProfile.id,
		};

		try {
			await createJobOffer(request).unwrap();

			onSuccess();
			resetForm();
		} catch (error) {
			toast.error("Failed to send hiring request. Please try again.");
		}
	};

	const resetForm = () => {
		setFormData({
			title: "",
			description: "",
			budget: "",
			budgetType: "fixed",
			area: "",
			timeline: "",
			attachments: [],
		});
		setErrors({});
	};

	const handleClose = () => {
		resetForm();
		onClose();
	};

	return (
		<Dialog open={isOpen} onOpenChange={handleClose}>
			<DialogContent className="sm:max-w-[600px] max-h-[90vh] overflow-y-auto">
				<DialogHeader>
					<DialogTitle className="flex items-center gap-3">
						<Briefcase className="h-5 w-5 text-indigo-600" />
						<div>
							<span>Hire {expertProfile.firstName}</span>
							<p className="text-sm font-normal text-gray-500 mt-1">
								{expertProfile.jobTitle} • @
								{expertProfile.username}
							</p>
						</div>
					</DialogTitle>
				</DialogHeader>

				{/* Expert Summary */}
				<div className="bg-secondary rounded-lg p-4 mb-4">
					<div className="flex items-center gap-3 mb-3">
						<div className="w-12 h-12 rounded-full bg-indigo-100 flex items-center justify-center">
							{expertProfile.profilePictureUrl ? (
								<img
									src={expertProfile.profilePictureUrl}
									alt={expertProfile.firstName}
									className="w-12 h-12 rounded-full object-cover"
								/>
							) : (
								<User className="w-6 h-6 text-indigo-600" />
							)}
						</div>
						<div>
							<h4 className="font-medium text-card-foreground">
								{expertProfile.firstName}
							</h4>
						</div>
					</div>
				</div>

				<form onSubmit={handleSubmit} className="space-y-4">
					<div className="space-y-2">
						<Label htmlFor="title">Project Title *</Label>
						<Input
							id="title"
							placeholder="e.g., Web Development for E-commerce Platform"
							value={formData.title}
							onChange={(e) =>
								handleInputChange("title", e.target.value)
							}
							className={errors.title ? "border-red-500" : ""}
						/>
						{errors.title && (
							<p className="text-sm text-red-500">
								{errors.title}
							</p>
						)}
					</div>

					<div className="space-y-2">
						<Label htmlFor="description">
							Project Description *
						</Label>
						<Textarea
							id="description"
							placeholder="Describe your project requirements, deliverables, and any specific needs..."
							value={formData.description}
							onChange={(e) =>
								handleInputChange("description", e.target.value)
							}
							rows={4}
							className={
								errors.description ? "border-red-500" : ""
							}
						/>
						{errors.description && (
							<p className="text-sm text-red-500">
								{errors.description}
							</p>
						)}
					</div>

					<div className="grid grid-cols-2 gap-4">
						<div className="space-y-2">
							<Label htmlFor="budget">Budget ($) *</Label>
							<div className="relative">
								<DollarSign className="absolute left-3 top-3 h-4 w-4 text-gray-400" />
								<Input
									id="budget"
									type="number"
									placeholder="0.00"
									value={formData.budget}
									onChange={(e) =>
										handleInputChange(
											"budget",
											e.target.value
										)
									}
									className={`pl-10 ${errors.budget ? "border-red-500" : ""}`}
									min="0"
									step="0.01"
								/>
							</div>
							{errors.budget && (
								<p className="text-sm text-red-500">
									{errors.budget}
								</p>
							)}
						</div>

						<div className="space-y-2">
							<Label htmlFor="budgetType">Budget Type</Label>
							<Select
								value={formData.budgetType}
								onValueChange={(value) =>
									handleInputChange("budgetType", value)
								}
							>
								<SelectTrigger>
									<SelectValue />
								</SelectTrigger>
								<SelectContent>
									<SelectItem value="fixed">
										Fixed Price
									</SelectItem>
									<SelectItem value="hourly">
										Hourly Rate
									</SelectItem>
									<SelectItem value="negotiable">
										Negotiable
									</SelectItem>
								</SelectContent>
							</Select>
						</div>
					</div>

					<div className="space-y-2">
						<Label htmlFor="area">Location *</Label>
						<div className="relative">
							<MapPin className="absolute left-3 top-3 h-4 w-4 text-gray-400" />
							<Input
								id="area"
								placeholder="e.g., Remote, New York, On-site, etc."
								value={formData.area}
								onChange={(e) =>
									handleInputChange("area", e.target.value)
								}
								className={`pl-10 ${errors.area ? "border-red-500" : ""}`}
							/>
						</div>
						{errors.area && (
							<p className="text-sm text-red-500">
								{errors.area}
							</p>
						)}
					</div>

					<div className="space-y-2">
						<Label htmlFor="timeline">Timeline *</Label>
						<div className="relative">
							<Clock className="absolute left-3 top-3 h-4 w-4 text-gray-400" />
							<Input
								id="timeline"
								placeholder="e.g., 2 weeks, 1 month, ASAP"
								value={formData.timeline}
								onChange={(e) =>
									handleInputChange(
										"timeline",
										e.target.value
									)
								}
								className={`pl-10 ${errors.timeline ? "border-red-500" : ""}`}
							/>
						</div>
						{errors.timeline && (
							<p className="text-sm text-red-500">
								{errors.timeline}
							</p>
						)}
					</div>

					{/* File Upload */}
					<div className="space-y-2">
						<Label>Attachments (Optional)</Label>
						<div
							className={`border-2 border-dashed rounded-lg p-4 transition-colors ${
								dragActive
									? "border-indigo-500 bg-indigo-50"
									: "border-gray-300"
							}`}
							onDragEnter={handleDrag}
							onDragLeave={handleDrag}
							onDragOver={handleDrag}
							onDrop={handleDrop}
						>
							<div className="text-center">
								<Upload className="mx-auto h-8 w-8 text-gray-400 mb-2" />
								<p className="text-sm text-gray-600">
									Drag & drop files here, or{" "}
									<label className="text-indigo-600 cursor-pointer hover:text-indigo-700">
										browse
										<input
											type="file"
											multiple
											accept=".pdf,.doc,.docx,.txt,.jpg,.jpeg,.png,.gif"
											onChange={(e) =>
												handleFileUpload(e.target.files)
											}
											className="hidden"
										/>
									</label>
								</p>
								<p className="text-xs text-gray-500 mt-1">
									Supports: PDF, DOC, TXT, Images (max 10MB, 5
									files)
								</p>
							</div>
						</div>

						{/* File List */}
						{formData.attachments.length > 0 && (
							<div className="space-y-2">
								{formData.attachments.map((file, index) => (
									<div
										key={index}
										className="flex items-center justify-between bg-secondary p-2 rounded"
									>
										<div className="flex items-center gap-2">
											<FileText className="h-4 w-4 text-gray-500" />
											<span className="text-sm text-card-foreground">
												{file.name}
											</span>
											<span className="text-xs text-gray-500">
												(
												{(
													file.size /
													1024 /
													1024
												).toFixed(1)}
												MB)
											</span>
										</div>
										<Button
											type="button"
											variant="ghost"
											size="sm"
											onClick={() => removeFile(index)}
										>
											<X className="h-4 w-4" />
										</Button>
									</div>
								))}
							</div>
						)}
					</div>

					<div className="flex gap-3 pt-6">
						<Button
							type="button"
							variant="outline"
							onClick={handleClose}
							className="flex-1"
							disabled={isLoading}
						>
							Cancel
						</Button>
						<Button
							type="submit"
							className="flex-1 bg-indigo-600 hover:bg-indigo-700"
							disabled={isLoading}
						>
							{isLoading ? "Sending..." : "Send Hiring Request"}
						</Button>
					</div>
				</form>
			</DialogContent>
		</Dialog>
	);
};

export default HiringModal;

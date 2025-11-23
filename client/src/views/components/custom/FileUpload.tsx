import React, { useCallback, useEffect, useRef, useState } from "react";
import {
	Trash2,
	Upload,
	Image as ImageIcon,
	Video,
	CheckCircle2,
	X,
} from "lucide-react";
import { Controller, useController, useFormContext } from "react-hook-form";
import ReactPlayer from "react-player";
import toast from "react-hot-toast";
import { motion, AnimatePresence } from "framer-motion";
import { MediaObject } from "@/features/media/types";
import { cn } from "@/lib/util/utils";

interface FileUploadProps {
	limit: number;
	multiple: boolean;
	name: string;
	setParentMediaList: (...args: any) => any;
}

// ? FileUpload Component
const FileUpload: React.FC<FileUploadProps> = ({
	limit,
	multiple,
	name,
	setParentMediaList,
}) => {
	// ? Form Context
	const {
		control,
		formState: { isSubmitting, errors },
	} = useFormContext();

	// ? State with useState()
	const { field } = useController({ name, control });
	const [singleFile, setSingleFile] = useState<File[]>([]);
	const [fileList, setFileList] = useState<File[]>([]);
	const [fileUrls, setFileUrls] = useState<string[]>([]);
	const [isDragging, setIsDragging] = useState(false);
	const [uploadedFiles, setUploadedFiles] = useState<Set<string>>(new Set());
	const [fileIds, setFileIds] = useState<string[]>([]); // Track unique IDs for each file
	const wrapperRef = useRef<HTMLDivElement>(null);

	// ? Toggle the drag state
	const onDragEnter = () => setIsDragging(true);
	const onDragLeave = () => setIsDragging(false);
	const onDrop = () => setIsDragging(false);

	// ? Image Upload Service
	const onFileDrop = useCallback(
		(e: React.SyntheticEvent<EventTarget>) => {
			const target = e.target as HTMLInputElement;
			if (!target.files) return;

			if (limit === 1) {
				const newFile = Object.values(target.files).map(
					(file: File) => file
				);
				if (singleFile.length >= 1) {
					toast.error("Only a single file allowed");
					return;
				}
				const fileId = `${newFile[0].name}-${newFile[0].size}-${Date.now()}`;
				setSingleFile(newFile);
				setFileIds([fileId]);
				field.onChange(newFile[0]);
				toast.success("File uploaded successfully!");
				setUploadedFiles(new Set([fileId]));
			}

			if (multiple) {
				const newFiles = Object.values(target.files).map(
					(file: File) => file
				);
				if (newFiles) {
					const updatedList = [...fileList, ...newFiles];
					if (updatedList.length > limit || newFiles.length > 3) {
						toast.error(`Maximum ${limit} files allowed`);
						return;
					}
					// Generate unique IDs for new files
					const newFileIds = newFiles.map(
						(file) =>
							`${file.name}-${file.size}-${Date.now()}-${Math.random()}`
					);
					const updatedIds = [...fileIds, ...newFileIds];

					setFileList(updatedList);
					setFileIds(updatedIds);
					field.onChange(updatedList);
					toast.success(
						`${newFiles.length} file(s) uploaded successfully!`
					);

					// Mark new files as uploaded
					const newUploadedSet = new Set(uploadedFiles);
					newFileIds.forEach((id) => {
						newUploadedSet.add(id);
					});
					setUploadedFiles(newUploadedSet);
				}
			}
		},
		[field, fileList, fileIds, limit, multiple, singleFile, uploadedFiles]
	);

	useEffect(() => {
		const urls = (multiple ? fileList : singleFile).map((file) =>
			URL.createObjectURL(file)
		);

		const mediaList: MediaObject[] = (multiple ? fileList : singleFile).map(
			(file) => ({
				url: URL.createObjectURL(file),
				file: file,
				// type: getType(file),
				type: file.type,
			})
		);

		setParentMediaList(mediaList);
		setFileUrls(urls);
	}, [singleFile, fileList, multiple, setParentMediaList]);

	// ? remove multiple images
	const fileRemove = (index: number) => {
		const updatedList = [...fileList];
		const updatedIds = [...fileIds];
		const removedId = updatedIds[index];

		updatedList.splice(index, 1);
		updatedIds.splice(index, 1);

		setFileList(updatedList);
		setFileIds(updatedIds);
		field.onChange(updatedList);

		// Remove from uploaded files set
		const newUploadedSet = new Set(uploadedFiles);
		newUploadedSet.delete(removedId);
		setUploadedFiles(newUploadedSet);

		toast.success("File removed");
	};

	// ? remove single image
	const fileSingleRemove = () => {
		setSingleFile([]);
		setFileIds([]);
		field.onChange(null);
		setUploadedFiles(new Set());
		toast.success("File removed");
	};

	// ? Calculate Size in KiloByte and MegaByte
	const calcSize = (size: number) => {
		return size < 1000000
			? `${Math.floor(size / 1000)} KB`
			: `${Math.floor(size / 1000000)} MB`;
	};

	// ? Reset the State
	useEffect(() => {
		if (isSubmitting) {
			setFileList([]);
			setSingleFile([]);
			setFileIds([]);
			setUploadedFiles(new Set());
		}
	}, [isSubmitting]);

	const currentFileCount = multiple ? fileList.length : singleFile.length;
	const hasFiles = currentFileCount > 0;

	// ? Actual JSX
	return (
		<div className="w-full space-y-1">
			{/* Upload Zone */}
			<motion.div
				animate={{
					scale: isDragging ? 1.02 : 1,
					opacity: !hasFiles ? [1, 0.95, 1] : 1,
				}}
				transition={{
					opacity: {
						duration: 3,
						repeat: !hasFiles ? Infinity : 0,
						ease: "easeInOut",
					},
					scale: { duration: 0.3 },
				}}
				className={cn(
					"relative rounded-2xl border-2 border-dashed transition-colors duration-300",
					isDragging
						? "border-primary bg-primary/5 shadow-lg shadow-primary/20"
						: "border-border hover:border-primary/50",
					"bg-linear-to-br from-muted/30 to-muted/50"
				)}
			>
				<div
					className={cn(
						"relative overflow-hidden rounded-2xl p-4 transition-all duration-300",
						isDragging && "bg-primary/10"
					)}
					ref={wrapperRef}
					onDragEnter={onDragEnter}
					onDragLeave={onDragLeave}
					onDrop={onDrop}
				>
					{/* Upload Icon & Text */}
					<div className="flex flex-col items-center justify-center gap-4 pointer-events-none">
						<motion.div
							animate={{ scale: isDragging ? 1.1 : 1 }}
							transition={{ duration: 0.3 }}
							className="relative"
						>
							<motion.div
								animate={{ opacity: isDragging ? 0.3 : 0 }}
								transition={{ duration: 0.3 }}
								className="absolute inset-0 rounded-full blur-xl bg-primary"
							/>
							<div
								className={cn(
									"relative p-2 rounded-full transition-colors duration-300",
									isDragging
										? "bg-primary text-primary-foreground"
										: "bg-primary/10 text-primary"
								)}
							>
								<Upload className="w-8 h-8" />
							</div>
						</motion.div>

						<div className="text-center space-y-2">
							<div className="font-semibold text-lg text-card-foreground">
								{isDragging ? (
									"Drop files here"
								) : (
									<>
										<span className="text-primary cursor-pointer hover:underline">
											Click to upload
										</span>
										{" or drag and drop"}
									</>
								)}
							</div>

							<div className="flex items-center justify-center gap-2 text-sm text-muted-foreground">
								<ImageIcon className="w-4 h-4" />
								<span>JPG, PNG</span>
								<span className="text-border">•</span>
								<Video className="w-4 h-4" />
								<span>MP4, WebM</span>
							</div>

							{multiple && (
								<div className="text-xs text-muted-foreground pt-1">
									{currentFileCount > 0 ? (
										<span className="text-primary font-medium">
											{currentFileCount} of {limit} files
											uploaded
										</span>
									) : (
										`Upload up to ${limit} files (images/videos)`
									)}
								</div>
							)}
						</div>
					</div>

					{/* Hidden File Input */}
					<Controller
						name={name}
						defaultValue=""
						control={control}
						render={({ field: { name, onBlur, ref } }) => (
							<input
								type="file"
								name={name}
								onBlur={onBlur}
								ref={ref}
								onChange={onFileDrop}
								multiple={multiple}
								accept="image/jpg, image/jpeg, image/png, video/mp4, video/webm, video/ogg"
								className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
								aria-label="File upload"
							/>
						)}
					/>
				</div>
			</motion.div>

			{/* Error Message */}
			<AnimatePresence>
				{errors[name] && (
					<motion.div
						initial={{ opacity: 0, x: -10 }}
						animate={{ opacity: 1, x: 0 }}
						exit={{ opacity: 0, x: -10 }}
						transition={{
							type: "spring",
							stiffness: 500,
							damping: 30,
						}}
						className="flex items-center gap-2 px-4 py-3 bg-destructive/10 border border-destructive/30 rounded-lg text-sm text-destructive"
					>
						<X className="w-4 h-4 shrink-0" />
						<span>{errors[name]?.message?.toString()}</span>
					</motion.div>
				)}
			</AnimatePresence>

			{/* File List */}
			<AnimatePresence>
				{hasFiles && (
					<motion.div
						initial={{ opacity: 0, y: 10 }}
						animate={{ opacity: 1, y: 0 }}
						exit={{ opacity: 0, y: 10 }}
						transition={{ duration: 0.3 }}
						className="space-y-3"
					>
						<div className="flex items-center justify-between px-1">
							<span className="text-sm font-medium text-muted-foreground">
								Uploaded Files ({currentFileCount})
							</span>
						</div>

						<div
							className={cn(
								"gap-3",
								multiple && currentFileCount > 1
									? "grid grid-cols-1 sm:grid-cols-2"
									: "flex flex-col"
							)}
						>
							{(multiple ? fileList : singleFile).map(
								(item, index) => {
									const fileType = item.type.split("/")[0];
									const isImage = fileType === "image";
									const isVideo = fileType === "video";
									const fileId = fileIds[index];
									const isUploaded =
										uploadedFiles.has(fileId);

									return (
										<motion.div
											key={fileId}
											initial={{ opacity: 0, y: -10 }}
											animate={{ opacity: 1, y: 0 }}
											exit={{
												opacity: 0,
												scale: 0.9,
											}}
											transition={{
												delay: index * 0.05,
												duration: 0.3,
											}}
											whileHover={{ scale: 1.02 }}
											className="group relative bg-card border border-border rounded-xl overflow-hidden hover:shadow-lg hover:border-primary/30 transition-colors duration-300"
										>
											{/* File Preview */}
											<div className="relative aspect-video bg-muted/30 overflow-hidden">
												{isImage ? (
													<>
														<img
															src={
																fileUrls[index]
															}
															alt={item.name}
															className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-110"
														/>
														<div className="absolute inset-0 bg-linear-to-t from-black/50 to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
													</>
												) : isVideo ? (
													<div className="relative w-full h-full">
														<ReactPlayer
															url={
																fileUrls[index]
															}
															width="100%"
															height="100%"
															light
															controls
														/>
													</div>
												) : null}
												{/* File Type Badge */}
												<div className="absolute top-2 left-2 px-2 py-1 bg-black/70 backdrop-blur-sm rounded-md text-xs font-medium text-white flex items-center gap-1">
													{isImage ? (
														<>
															<ImageIcon className="w-3 h-3" />
															<span>Image</span>
														</>
													) : (
														<>
															<Video className="w-3 h-3" />
															<span>Video</span>
														</>
													)}
												</div>
												{/* Success Indicator */}
												<AnimatePresence>
													{isUploaded && (
														<motion.div
															initial={{
																scale: 0,
																opacity: 0,
															}}
															animate={{
																scale: 1,
																opacity: 1,
															}}
															exit={{
																scale: 0,
																opacity: 0,
															}}
															transition={{
																type: "spring",
																stiffness: 500,
																damping: 30,
															}}
															className="absolute top-2 right-2 p-1 bg-green-500 rounded-full"
														>
															<CheckCircle2 className="w-4 h-4 text-white" />
														</motion.div>
													)}
												</AnimatePresence>{" "}
												{/* Delete Button */}
												<button
													onClick={() => {
														if (multiple) {
															fileRemove(index);
														} else {
															fileSingleRemove();
														}
													}}
													className="absolute top-2 right-2 p-2 bg-destructive/90 hover:bg-destructive text-white rounded-lg opacity-0 group-hover:opacity-100 transition-all duration-200 hover:scale-110"
													aria-label="Remove file"
												>
													<Trash2 className="w-4 h-4" />
												</button>
											</div>

											{/* File Info */}
											<div className="p-3 space-y-1">
												<div
													className="text-sm font-medium text-card-foreground truncate"
													title={item.name}
												>
													{item.name}
												</div>
												<div className="flex items-center gap-2 text-xs text-muted-foreground">
													<span className="px-2 py-0.5 bg-muted rounded-md font-medium">
														{calcSize(item.size)}
													</span>
													{isUploaded && (
														<span className="flex items-center gap-1 text-green-600">
															<CheckCircle2 className="w-3 h-3" />
															Uploaded
														</span>
													)}
												</div>
											</div>
										</motion.div>
									);
								}
							)}
						</div>
					</motion.div>
				)}
			</AnimatePresence>
		</div>
	);
};

export default FileUpload;

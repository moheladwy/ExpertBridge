import React, { useCallback, useEffect, useRef, useState } from "react";
import { Trash2 } from "lucide-react";
import { Controller, useController, useFormContext } from "react-hook-form";
import ReactPlayer from "react-player";
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
	const wrapperRef = useRef<HTMLDivElement>(null);

	// ? Toggle the dragover class
	const onDragEnter = () => wrapperRef.current?.classList.add("dragover");
	const onDragLeave = () => wrapperRef.current?.classList.remove("dragover");

	// ? Image Upload Service
	const onFileDrop = useCallback(
		(e: React.SyntheticEvent<EventTarget>) => {
			const target = e.target as HTMLInputElement;
			if (!target.files) return;

			if (limit === 1) {
				const newFile = Object.values(target.files).map(
					(file: File) => file
				);
				if (singleFile.length >= 1)
					return alert("Only a single image allowed");
				setSingleFile(newFile);
				field.onChange(newFile[0]);
			}

			if (multiple) {
				const newFiles = Object.values(target.files).map(
					(file: File) => file
				);
				if (newFiles) {
					const updatedList = [...fileList, ...newFiles];
					if (updatedList.length > limit || newFiles.length > 3) {
						return alert(`Image must not be more than ${limit}`);
					}
					setFileList(updatedList);
					field.onChange(updatedList);
				}
			}
		},
		[field, fileList, limit, multiple, singleFile]
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
	const fileRemove = (file: File) => {
		const updatedList = [...fileList];
		updatedList.splice(fileList.indexOf(file), 1);
		setFileList(updatedList);
	};

	// ? remove single image
	const fileSingleRemove = () => {
		setSingleFile([]);
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
		}
	}, [isSubmitting]);

	// ? Actual JSX
	return (
		<>
			<div className="flex">
				<div className="bg-card rounded-xl border border-border shadow-lg p-4 hover:border-primary/50 transition-all duration-200">
					<div
						className="flex justify-center items-center relative w-full h-[25vh] border-2 border-dashed border-primary/30 rounded-xl p-5 bg-muted/30 hover:bg-muted/50 transition-colors"
						ref={wrapperRef}
						onDragEnter={onDragEnter}
						onDragLeave={onDragLeave}
						onDrop={onDragLeave}
					>
						<div className="flex justify-center p-1 text-center">
							<div className="font-bold text-card-foreground max-md:text-sm max-md:w-full">
								{limit > 1
									? "Browse to upload files"
									: "Browse to upload file"}
							</div>

							<div className="max-md:hidden">
								<div className="text-muted-foreground max-md:text-sm">
									Supported Files
								</div>
								<div className="text-muted-foreground max-md:text-sm">
									JPG, JPEG, PNG, MP4
								</div>
							</div>
						</div>
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
									style={{
										opacity: 0,
										position: "absolute",
										top: 0,
										left: 0,
										width: "100%",
										height: "100%",
										cursor: "pointer",
									}}
								/>
							)}
						/>
					</div>
				</div>

				<div
					className={cn(
						"text-center my-1",
						errors[name] && "text-red-500"
					)}
				>
					{(errors[name] ? errors[name].message : "")?.toString()}
				</div>

				{(fileList.length > 0 || singleFile.length > 0) && (
					<div className="ml-3">
						<div className="space-y-2 my-2">
							{(multiple ? fileList : singleFile).map(
								(item, index) => {
									const fileType = item.type.split("/")[0]; // 'image' or 'video'
									const isImage = fileType === "image";
									const isVideo = fileType === "video";

									return (
										<div
											key={index}
											className="relative bg-card border border-border rounded-xl p-3 hover:shadow-md transition-all duration-200"
										>
											<div className="flex items-center justify-between w-full">
												<div className="flex items-center gap-3">
													{isImage ? (
														<img
															src={
																fileUrls[index]
															}
															alt="upload"
															style={{
																height: "3.5rem",
																objectFit:
																	"contain",
															}}
															className="rounded-lg"
														/>
													) : isVideo ? (
														<ReactPlayer
															url={
																fileUrls[index]
															}
															width="100px"
															height="56px"
															controls
														/>
													) : null}

													<div className="flex-1">
														<div className="text-sm font-medium text-card-foreground max-xl:hidden">
															{item.name}
														</div>
														<div className="text-xs text-muted-foreground max-xl:hidden">
															{calcSize(
																item.size
															)}
														</div>
													</div>
												</div>

												<button
													onClick={() => {
														if (multiple) {
															fileRemove(item);
														} else {
															fileSingleRemove();
														}
													}}
													className="max-lg:w-7 max-md:w-4 text-destructive hover:text-destructive/80 transition-colors p-2 hover:bg-destructive/10 rounded-lg"
												>
													<Trash2 className="max-lg:w-5 w-5 h-5" />
												</button>
											</div>
										</div>
									);
								}
							)}
						</div>
					</div>
				)}
			</div>
		</>
	);
};

export default FileUpload;

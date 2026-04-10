import { merge } from "../lib/utils"

export const Button = ({ onClick, children, className, ...props }) => {
  return (
    <button onClick={onClick} className={merge("btn", className)} {...props}>
      {children}
    </button>
  )
}